using BooksBot.API.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BooksBot.API.Constants.Enums;

namespace BooksBot.API.Data.Repositories
{
    public interface IShouldCrawlEanRepository
    {
        IQueryable<ShouldCrawlEan> ShouldCrawlEans { get; }
        Task<List<ShouldCrawlEan>> GetShouldCrawlEans();
        Task UpdateRangeShouldCrawlEan(List<ShouldCrawlEan> crawlEans);
        Task<List<ShouldCrawlEan>> GetShouldCrawlEansByShortName(SourceWebSiteShortNameEnum sourceWebSiteShortNameEnum);
    }
}
