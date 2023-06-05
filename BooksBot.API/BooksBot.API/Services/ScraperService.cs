using AutoMapper;
using BooksBot.API.Configurations;
using BooksBot.API.Constants;
using BooksBot.API.Data.Entities;
using BooksBot.API.Data.Repositories;
using BooksBot.API.Models;
using BooksBot.API.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static BooksBot.API.Constants.Enums;

namespace BooksBot.API.Services
{
    public class ScraperService : IScraperService
    {
        private readonly IMapper _mapper;

        private readonly IBookDataRepository _bookDataRepository;

        private readonly ICrawlSourceRepository _crawlSourceRepository;

        private readonly IShouldCrawlEanRepository _shouldCrawlEanRepository;
        private readonly ISourceWebsiteDataRepository _SourceWebsiteDataRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<ScraperService> _logger;

        private readonly AppConfiguration _appConfig;

        private List<string> failedRequestUrls = null;
        private List<string> invalidUrls = null;
        private List<string> successfulRequestUrls = null;
        private List<CrawlSource> usedCrawlSource = null;
        
        public ScraperService(IMapper mapper,
                              IBookDataRepository bookDataRepository,
                              ICrawlSourceRepository crawlSourceRepository,
                              IUnitOfWork unitOfWork,
                              ILogger<ScraperService> logger,
                              IShouldCrawlEanRepository shouldCrawlEanRepository,
                              AppConfiguration appConfig,
                              ISourceWebsiteDataRepository sourceWebsiteDataRepository)
        {
            _mapper = mapper;
            _bookDataRepository = bookDataRepository;
            _crawlSourceRepository = crawlSourceRepository;
            _shouldCrawlEanRepository = shouldCrawlEanRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _appConfig = appConfig;
            _SourceWebsiteDataRepository = sourceWebsiteDataRepository;
        }


        public async Task<CrawlResponseModel> ScrapBooksUrlsAndReturnResult()
        {
            var crawlSources = await FetchCrawlSources();
            //crawlSources = crawlSources.Take(2).ToList();
            await SetCrawlingInProgress(crawlSources);

            var crawlTasks = new List<Task>();
            var UsedcrawlTasks = new List<Task>();
            var bookList = new List<WobBookModel>();

            failedRequestUrls = new List<string>();
            successfulRequestUrls = new List<string>();
            invalidUrls = new List<string>();
            usedCrawlSource = new List<CrawlSource>();
            //preparing the tasks to be executed asyncronously
            foreach (var source in crawlSources)
            {
                //encoding the target url as per the requirement of ScrapingBee

                //var fullUrl = $"{source.Url}";
                // await Task.Delay(_appConfig.DelayTimeInSeconds * 1000);
                var responseTask = CallUrlAndValidateHtml(source);
                crawlTasks.Add(responseTask);
            }

            //process it further if any of the tasks has completed
            while (crawlTasks.Any())
            {
                Task finishedTask = await Task.WhenAny(crawlTasks);
                var htmlResponse = ((Task<HtmlResponseModel>)finishedTask).Result;
                if (htmlResponse.Html != null)
                {
                    ParseHtmlAndPopulateBookList(bookList, htmlResponse, htmlResponse.CrawlSource.SourceWebsite.SourceWebSiteShortNameEnum);
                }
                crawlTasks.Remove(finishedTask);
            }

            if (usedCrawlSource.Any())
            {
                //string OrgURL = string.Empty;
                foreach (var source in usedCrawlSource)
                {
                    var UsedresponseTask = CallUrlAndValidateHtml(source);
                    UsedcrawlTasks.Add(UsedresponseTask);
                }
                while (UsedcrawlTasks.Any())
                {
                    Task finishedTasknew = await Task.WhenAny(UsedcrawlTasks);
                    var htmlResponsenew = ((Task<HtmlResponseModel>)finishedTasknew).Result;
                    if (htmlResponsenew.Html != null)
                        ParseHtmlAndPopulateBookList(bookList, htmlResponsenew, htmlResponsenew.CrawlSource.SourceWebsite.SourceWebSiteShortNameEnum);
                    UsedcrawlTasks.Remove(finishedTasknew);
                }
            }

            await InsertBookItems(bookList);
            await UpdateCrawlSources(crawlSources);
            await _unitOfWork.Commit();

            var responseModel = new CrawlResponseModel()
            {
                CrawledBooks = bookList,
                FailedUrls = failedRequestUrls,
                InvalidUrls = invalidUrls,
                SuccessfulUrls = successfulRequestUrls,
                CrawledBooksCount = bookList.Count
            };
            return responseModel;
        }

        public async Task ResetLongRunningCrawlProcesses()
        {
            var longRunningSources = _crawlSourceRepository.CrawlSources.Where(cs => cs.Status == StringConstants.InProgress && cs.CrawlStartTime <= DateTime.UtcNow.AddHours(-(_appConfig.ResetRunningCrawlProcessInHours))).ToList();
            longRunningSources.ForEach(cs => { cs.Status = null; cs.CrawlStartTime = null; });
            await _crawlSourceRepository.UpdateRangeAsync(longRunningSources);
            await _unitOfWork.Commit();
        }

        //public async Task<CrawlSourcesSearchResult> SearchCrawlSourcesFromEan()
        //{
        //     var eanList = await _shouldCrawlEanRepository.GetShouldCrawlEans();
        //    //var eanList = new List<string>();
            
        //    CrawlSourcesSearchResult result = new();
        //    var CrawlSourcesFound = new List<string>();
        //    try
        //    {
        //        if (eanList.Any())
        //        {
        //            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
        //            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
        //            {
        //                Headless = false
        //            });

        //            var page = await browser.NewPageAsync();
        //            page.DefaultTimeout = 0;
        //            var navigation = new NavigationOptions
        //            {
        //                Timeout = 0,
        //                WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded }
        //            };

        //            var searchSelector = ".input-group input";


        //            await page.SetRequestInterceptionAsync(true);

        //            // disable images to download
        //            page.Request += (sender, e) =>
        //            {
        //                if (e.Request.ResourceType == ResourceType.Image)
        //                    e.Request.AbortAsync();
        //                else
        //                    e.Request.ContinueAsync();
        //            };


        //            await page.GoToAsync("https://www.wob.com/en-gb", navigation);
        //            await page.WaitForSelectorAsync(".banner-actions-container");
        //            await page.ClickAsync("#onetrust-accept-btn-handler");
        //            await page.WaitForSelectorAsync(searchSelector);
        //            await page.FocusAsync(searchSelector);
        //            await page.Keyboard.TypeAsync(eanList.First());
        //            await page.Keyboard.PressAsync("Enter");
        //            //await page.ClickAsync(".btn");
        //            //await page.ClickAsync(".btn");
        //            await page.WaitForNavigationAsync();
        //            if (!page.Url.Contains($"search={eanList.First()}"))
        //            {
        //                CrawlSourcesFound.Add(page.Url);
        //                result.CrawlSourcesFound.Add(eanList.First());
        //                // eanList.First().CrawlSourceFound = true;
        //            }
        //            else
        //                result.CrawlSourcesNotFound.Add(eanList.First());

        //            //eanList.First().CrawlSourceSearched = true;

        //            foreach (var ean in eanList.Skip(1))
        //            {
        //                await page.FocusAsync(searchSelector);
        //                await page.ClickAsync(searchSelector, new ClickOptions { ClickCount = 3 });
        //                await page.Keyboard.TypeAsync(ean);
        //                await page.Keyboard.PressAsync("Enter");
        //                // await page.ClickAsync(".btn");
        //                await page.WaitForNavigationAsync();
        //                if (!page.Url.Contains($"search={ean}"))
        //                {
        //                    CrawlSourcesFound.Add(page.Url);
        //                    result.CrawlSourcesFound.Add(ean);
        //                    //ean.CrawlSourceFound = true;
        //                }
        //                else
        //                    result.CrawlSourcesNotFound.Add(ean);

        //                // ean.CrawlSourceSearched = true;
        //            }
        //            await browser.CloseAsync();

        //            await InsertCrawlSourceDataToDB(CrawlSourcesFound);
        //            //await _shouldCrawlEanRepository.UpdateRangeShouldCrawlEan(eanList);
        //            await _unitOfWork.Commit();

        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //    }

        //    return result;
        //}


        public async Task<List<CrawlSourceFound>> SearchCrawlSourcesFromEan()
        {
            var eanList = await _shouldCrawlEanRepository.GetShouldCrawlEansByShortName(SourceWebSiteShortNameEnum.AB);
            //var eanList = await _shouldCrawlEanRepository.GetShouldCrawlEans();
            //eanList = eanList.Take(40).ToList();
            List<CrawlSourceFound> result = new();
            var crawlTasks = new List<Task>();
            try
            {
                if (eanList.Any())
                {

                    foreach (var ean in eanList)
                    {
                        var responseTask = CallSearchEanUrl(ean);
                        crawlTasks.Add(responseTask);
                    }

                    while (crawlTasks.Any())
                    {
                        Task finishedTask = await Task.WhenAny(crawlTasks);
                        var htmlResponse = ((Task<HtmlResponseModelEAN>)finishedTask).Result;
                        //if (!string.IsNullOrEmpty(htmlResponse))

                        if (htmlResponse != null && !string.IsNullOrEmpty(htmlResponse.response))
                        {
                            //var productUrl = HtmlParser.ParseProductUrl(htmlResponse);
                            ParserFactory pf = new ParserFactory();
                            IHtmlParser Parser = pf.GetParser(htmlResponse.shouldCrawlEan.SourceWebSiteShortNameEnum);
                            var productUrl = Parser.ParseProductUrl(htmlResponse.response);

                            if (string.IsNullOrEmpty(productUrl))
                            {
                                var searchedEan = eanList.First(a => htmlResponse.shouldCrawlEan.Ean == a.Ean && htmlResponse.shouldCrawlEan.SourceWebSiteShortNameEnum == a.SourceWebSiteShortNameEnum); //.ToString().Contains($"https://www.wob.com/en-us/category/all?search={a.Ean}"));
                                searchedEan.CrawlSourceFound = false;
                            }
                            else
                                result.Add(new CrawlSourceFound() { CrawlSource = productUrl, SourceWebSiteShortNameEnum = htmlResponse.shouldCrawlEan.SourceWebSiteShortNameEnum });
                            
                        }
                        crawlTasks.Remove(finishedTask);
                    }
                    await InsertCrawlSourceDataToDB(result);
                    await _shouldCrawlEanRepository.UpdateRangeShouldCrawlEan(eanList);
                    await _unitOfWork.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return result;
        }


        #region Private Methods

        private async Task InsertCrawlSourceDataToDB(List<CrawlSourceFound> urlList)
        {
            Guid siteId;
            string currency;
            if (urlList.Any())
            {
                List<CrawlSource> crawlSources = new();

                var DistinctEnums = urlList.Select(a => a.SourceWebSiteShortNameEnum).ToList().Distinct();

                foreach (var enums in DistinctEnums)
                {
                    var result = await _SourceWebsiteDataRepository.GetSourceWebsiteByShortName(enums);
                    siteId = result.Id;
                    currency = result.Currency; //"GBP";
                    urlList.Where(x => x.SourceWebSiteShortNameEnum == enums).ToList().ForEach(cs => crawlSources.Add(new CrawlSource() { Id = Guid.NewGuid(), SourceWebsiteId = siteId, Url = cs.CrawlSource, Priority = 1, ShouldCrawl = true, CrawlCounter = 0, Currency = currency, PageType = PageTypeEnum.Detail, Status = null }));
                    await _crawlSourceRepository.AddRangeAsync(crawlSources);
                }
                
           }
        }


        private void ParseHtmlAndPopulateBookList(List<WobBookModel> bookList, HtmlResponseModel htmlResponse, SourceWebSiteShortNameEnum sourceWebSiteShortNameEnum)
        {
            ParserFactory pf = new ParserFactory();
            IHtmlParser Parser =   pf.GetParser(sourceWebSiteShortNameEnum);
            var responseData =  Parser.IdentifyExactParser(htmlResponse);
            if (responseData.IsUsed)
            {
                htmlResponse.CrawlSource.UsedUrl = responseData.UsedURL;
                usedCrawlSource.Add(htmlResponse.CrawlSource);
            }

            else
            {
                responseData.bookData.ForEach(book => book.CrawlSourceId = htmlResponse.CrawlSource.Id);
                bookList.AddRange(responseData.bookData);
            }
            Parser.Dispose();
        }

        //method to call the URL and then validate the response HTML
        private async Task<HtmlResponseModel> CallUrlAndValidateHtml(CrawlSource source)
        {
            var response = new HtmlResponseModel();
            var responseHtml = await CallUrl(source);
            var pageNotFound = !string.IsNullOrEmpty(responseHtml) ? AppContants.NotFoundMessagesArray.Any(item => responseHtml.Contains(item)) : false;
            //sometimes if the url of WOB(can be anyother site as well) is incorrect, it return html but with 'not found' message. So its a failed request
            if (responseHtml != null)
            {
                //var fullUrl = $"{_appConfig.ScrappingBeeClientUrl}{HttpUtility.UrlEncode(source.Url)}{_appConfig.ExtraQueryParams}";
                if (pageNotFound)
                {
                    responseHtml = null;
                    //removing ScrappingBeeClientURL from full url bcz it contains API key that is private
                    invalidUrls.Add(source.Url);
                    _logger.LogError($"{MessageStrings.PageNotFound}: {source.Url}");
                }
                //it means the url is successfully got hit
                else
                    successfulRequestUrls.Add(string.IsNullOrEmpty(source.UsedUrl) ? source.Url : source.UsedUrl);

            }

            response.Html = responseHtml;
            response.CrawlSource = source;
            return response;
        }

        //method to call an external url
        private async Task<string> CallUrl(CrawlSource source)
        {
            string response = null;
            var fullUrl = $"{_appConfig.ScrappingBeeClientUrl}{HttpUtility.UrlEncode(string.IsNullOrEmpty(source.UsedUrl) ? source.Url : source.UsedUrl)}{_appConfig.ExtraQueryParams}";
            try
            {
                HttpClient client = new();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = TimeSpan.FromSeconds(500);
                string s = await client.GetStringAsync(fullUrl);
                response = HttpUtility.HtmlDecode(s);
            }
            catch (Exception ex)
            {
                if (AppContants.NotFoundMessagesArray.Any(item => ex.ToString().Contains(item)) && source.CrawlCounter == 0)
                    invalidUrls.Add(GetTargetWebsiteUrlOnly(fullUrl));
                else
                    failedRequestUrls.Add(GetTargetWebsiteUrlOnly(fullUrl));

                _logger.LogError(ex.ToString());
                _logger.LogError($"{MessageStrings.RequestFailed}: {GetTargetWebsiteUrlOnly(fullUrl)}");
            }

            return response;
        }


        private async Task<HtmlResponseModelEAN> CallSearchEanUrl(ShouldCrawlEan ean)
        {
            ParserFactory pf = new ParserFactory();
            IHtmlParser Parser = pf.GetParser(ean.SourceWebSiteShortNameEnum);
            var response = new HtmlResponseModelEAN();
            //string response = null;
            var url = Parser.EanSearchURL;
            pf.Dispose();
            var fullUrl = $"{_appConfig.ScrappingBeeClientUrl}{HttpUtility.UrlEncode($"{url}{ean.Ean}")}{_appConfig.ExtraQueryParams}";
            try
            {
                HttpClient client = new();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = TimeSpan.FromSeconds(1000);
                response.response = await client.GetStringAsync(fullUrl);
                ean.CrawlSourceFound = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _logger.LogError($"{MessageStrings.RequestFailed}: {GetTargetWebsiteUrlOnly(fullUrl)}");
            }
            ean.CrawlSourceSearched = true;
            response.shouldCrawlEan = ean;
            return response;
        }

        private async Task InsertBookItems(List<WobBookModel> bookModels)
        {
            var bookDataList = bookModels.Select(c => _mapper.Map<BookData>(c)).ToList();

            var bookEANList = bookDataList.Select(t => t.EAN).ToList();
            var bookCrawlSourceIdList = bookDataList.Select(t => t.CrawlSourceId).Where(csId => csId != Guid.Empty).ToList();

            var mostRecentBookRecords = await _bookDataRepository.GetBookPricesFromEan(bookEANList);

            foreach (var book in bookDataList)
            {
                book.Id = Guid.NewGuid();
                //initially, in phase one we will keep the Quantity = 1, and InStock = true
                book.Quantity = book.Quantity; // book.Quantity == 0 ? 10 : book.Quantity;
                book.InStock = book.Quantity > 0;
                var lastFetchedPrice = mostRecentBookRecords.FirstOrDefault(b => b.EAN.Equals(book.EAN) && b.CrawlSourceId == book.CrawlSourceId)?.Price;

                if (lastFetchedPrice.HasValue && book.Price != lastFetchedPrice.Value)
                    book.Difference = Math.Round(book.Price - lastFetchedPrice.Value, 2);
            }

            await _bookDataRepository.AddRangeAsync(bookDataList);
        }


        private async Task UpdateCrawlSources(List<CrawlSource> crawlSources)
        {
            var successfulyCrawledSources = crawlSources.Where(t => successfulRequestUrls.Any(x => x.Contains(t.Url))).ToList();

            successfulyCrawledSources.ForEach(cs =>
            {
                cs.CrawlCounter += 1;
                cs.LastCrawled = DateTime.UtcNow;
                cs.Status = StringConstants.Success;
            });

            crawlSources.Where(t => invalidUrls.Any(x => x.Contains(t.Url))).ToList().ForEach(cs => cs.Status = StringConstants.InvalidUrl);
            crawlSources.Where(t => failedRequestUrls.Any(x => x.Contains(t.Url))).ToList().ForEach(cs => { cs.Status = StringConstants.Failed; cs.Priority += 1; });

            await _crawlSourceRepository.UpdateRangeAsync(crawlSources);
        }


        private async Task<List<CrawlSource>> FetchCrawlSources()
        {
            return await _crawlSourceRepository.CrawlSources.Where(cs => cs.ShouldCrawl && (cs.Status == null || cs.Status == StringConstants.Success))
                                                                      .OrderBy(cs => cs.CrawlCounter)
                                                                      .ThenBy(cs => cs.Priority)
                                                                      .Take(_appConfig.UrlCount)
                                                                      .ToListAsync();
        }


        private async Task SetCrawlingInProgress(List<CrawlSource> crawlSources)
        {
            crawlSources.ForEach(source => { source.Status = StringConstants.InProgress; source.CrawlStartTime = DateTime.UtcNow; });
            await _crawlSourceRepository.UpdateRangeAsync(crawlSources);
            await _unitOfWork.Commit();
        }

        private string GetTargetWebsiteUrlOnly(string fullUrl)
        {
            return HttpUtility.UrlDecode(fullUrl).Replace(_appConfig.ScrappingBeeClientUrl, "").Replace(_appConfig.ExtraQueryParams, "");
        }

        public async Task<List<UrlStatus>> GetUrlsStatusCrawledIn24Hours()
        {
            return await _crawlSourceRepository.GetUrlsStatusCrawledIn24Hours();
           // return _mapper.Map<List<UrlStatus>>(sources);
        }

        #endregion
    }
}
