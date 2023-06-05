using BooksBot.API.Data.Entities;
using BooksBot.API.Extensions;
using BooksBot.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BooksBot.API.Constants.Enums;

namespace BooksBot.API.Data.Repositories
{
    public interface ISourceWebsiteDataRepository
    {
        IQueryable<SourceWebsite> SourceWebsite { get; }
        Task AddRangeAsync(List<SourceWebsite> entities);
        Task<SourceWebsite> GetSourceWebsiteByShortName(SourceWebSiteShortNameEnum sourceWebSiteShortNameEnum);

        //Task<List<BookData>> GetBooksPricesFromWebsites(int pageNumber, int pageSize, string searchText);

    }
}