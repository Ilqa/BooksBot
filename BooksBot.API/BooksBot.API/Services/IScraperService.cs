using BooksBot.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooksBot.API.Services
{
    public interface IScraperService
    {
        //Task<List<string>> ScrapAndReturnResult();
        //Task<WobBookModel> ScrapBooksUrlAndReturnResult(string url);
        Task<CrawlResponseModel> ScrapBooksUrlsAndReturnResult();
        Task ResetLongRunningCrawlProcesses();
        //Task<CrawlSourcesSearchResult> SearchCrawlSourcesFromEan();
        Task<List<UrlStatus>> GetUrlsStatusCrawledIn24Hours();
        Task<List<CrawlSourceFound>> SearchCrawlSourcesFromEan();
    }
}
