using BooksBot.API.Models;
using BooksBot.API.Services;
using BooksBot.API.Utilities;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooksBot.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ScraperController : ControllerBase
    {

        private readonly IScraperService _scraperService;
        private readonly ILogger<ScraperController> _logger;
        public ScraperController(
            IScraperService scraperService,
            ILogger<ScraperController> logger
            )
        {
            _scraperService = scraperService;
            _logger = logger;
        }

        /// <summary>
        /// This Endpoint will fetch the urls saved in database, scrap them and then save the scraped book data into the database again
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("RunScrapingEngine")]
        public async Task<CrawlResponseModel> RunScrapingEngineAsync()
        {
            return await _scraperService.ScrapBooksUrlsAndReturnResult();
        }


        [HttpGet]
        [ActionName("ResetLongRunningCrawlProcesses")]
        public async Task ResetLongRunningCrawlProcesses()
        {
            await _scraperService.ResetLongRunningCrawlProcesses();
        }

        
        //[HttpGet]
        //[ActionName("SearchCrawlSourcesFromEan")]
        //public async Task<CrawlSourcesSearchResult> SearchCrawlSourcesFromEan()
        //{
        //    return await _scraperService.SearchCrawlSourcesFromEan();
        //}

        [HttpGet]
        [ActionName("GetUrlsStatusCrawledIn24Hours")]
        public async Task<List<UrlStatus>> GetUrlsStatusCrawledIn24Hours()
        {
            return await _scraperService.GetUrlsStatusCrawledIn24Hours();
        }

        [HttpGet]
        [ActionName("SearchCrawlSourcesFromEan")]
        public async Task<List<CrawlSourceFound>> SearchCrawlSourcesFromEan() 
        {
            return await _scraperService.SearchCrawlSourcesFromEan();
        }

    }
}
