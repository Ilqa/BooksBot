using BooksBot.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooksBot.API.Services
{
    public interface IBookDataService
    {
        Task<PaginatedBooksResult> GetBooksWithPricesList(int pageNumber, int pageSize, string searchText, bool searchTextChanged);

        Task<DownloadableBookData> GetCompleteDownloadableBooksWithPricesList();

        Task<DownloadableBookData> GetDownloadableBooksDataFromLastHour();

        Task<string> ArchiveBookData();

        Task<List<BookWithPriceList>> GetBookPricesFromEan(List<string> eanList);
        Task<CrawlStatus> GetCrawlStatus();
    }
}
