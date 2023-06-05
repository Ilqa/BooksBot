using BooksBot.API.Data.Entities;
using BooksBot.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksBot.API.Data.Repositories
{
    public interface ICrawlSourceRepository
    {
        IQueryable<CrawlSource> CrawlSources { get; }
        Task UpdateRangeAsync(List<CrawlSource> crawlSources);

        Task AddRangeAsync(List<CrawlSource> entities);

        //Task<List<CrawlSource>> LongRunningCrawlProcess { get; }

        Task<List<UrlStatus>> GetUrlsStatusCrawledIn24Hours();
    }
}
