using BooksBot.API.Models;
using BooksBot.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooksBot.API.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookDataController : Controller
    {

        private readonly IBookDataService _bookDataService;

        public BookDataController(IBookDataService bookDataService)
        {
            _bookDataService = bookDataService;
        }

        /// <summary>
        /// This end point will get the latest prices of books from dofferent websites
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetBooksPricesFromWebsites")]
        public async Task<PaginatedBooksResult> GetBooksPricesFromWebsites(int pageNumber = 1, int pageSize= 10, string searchText ="", bool searchTextChanged = false)
        {
            return await _bookDataService.GetBooksWithPricesList(pageNumber, pageSize, searchText, searchTextChanged);
        }

        //[HttpGet]
        //[ActionName("GetCompleteDownloadableBooksWithPricesList")]
        //public async Task<DownloadableBookData> GetCompleteDownloadableBooksWithPricesList()
        //{
        //    return await _bookDataService.GetCompleteDownloadableBooksWithPricesList();
        //}

        [HttpGet]
        [ActionName("GetDownloadableBooksDataFromLastHour")]
        public async Task<DownloadableBookData> GetDownloadableBooksDataFromLastHour()
        {
            return await _bookDataService.GetDownloadableBooksDataFromLastHour();
        }

        [AllowAnonymous]
        [HttpPost]
        [ActionName("GetBookPricesFromEan")]
        public async Task<List<BookWithPriceList>> GetBookPricesFromEan(List<string> eanList)
        {
            return await _bookDataService.GetBookPricesFromEan(eanList);
        }

        [HttpGet]
        [ActionName("GetCrawlStatus")]
        public async Task<CrawlStatus> GetCrawlStatus()
        {
            return await _bookDataService.GetCrawlStatus();
        }

        [AllowAnonymous]
        [HttpGet]
        [ActionName("ArchiveBookData")]
        public async Task<string> ArchiveBookData()
        {
            return await _bookDataService.ArchiveBookData();
        }
    }
}
