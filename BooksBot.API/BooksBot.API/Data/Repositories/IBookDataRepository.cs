using BooksBot.API.Data.Entities;
using BooksBot.API.Extensions;
using BooksBot.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksBot.API.Data.Repositories
{
    public interface IBookDataRepository
    {
        IQueryable<BookData> Books { get; }
        Task AddRangeAsync(List<BookData> entities);

        Task<List<BookData>> GetBooksPricesFromWebsites(int pageNumber, int pageSize, string searchText);

        Task<List<BookData>> GetAllBooksPricesFromWebsites();

        Task<List<BookData>> GetBooksPricesFromLastHour();

        Task<int> GetTotalBooksCount(string searchText);
        Task<string> ArchiveBookData();
        Task<List<BookData>> GetBookPricesFromEan(List<string> eanList);
        Task<CrawlStatus> GetCrawlStatus();
    }
}