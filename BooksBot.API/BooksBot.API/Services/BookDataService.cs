using AutoMapper;
using BooksBot.API.Constants;
using BooksBot.API.Data.Entities;
using BooksBot.API.Data.Repositories;
using BooksBot.API.Extensions;
using BooksBot.API.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BooksBot.API.Services
{
    public class BookDataService : IBookDataService
    {
        private readonly IMapper _mapper;

        private readonly IBookDataRepository _bookDataRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<BookDataService> _logger;

        public BookDataService(IMapper mapper,
                                 IBookDataRepository bookDataRepository,
                                IUnitOfWork unitOfWork,
                                ILogger<BookDataService> logger)
        {
            _mapper = mapper;
            _bookDataRepository = bookDataRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string> ArchiveBookData() => await _bookDataRepository.ArchiveBookData();

        public async Task<PaginatedBooksResult> GetBooksWithPricesList(int pageNumber, int pageSize, string searchText, bool searchTextChanged)
        {
            PaginatedBooksResult paginatedBooksResult = new() { CurrentPage = pageNumber, PageSize = pageSize };
            List<BookData> books = new();

            if (searchTextChanged)
            {
                var countTask = _bookDataRepository.GetTotalBooksCount(searchText);
                var booksTask = _bookDataRepository.GetBooksPricesFromWebsites(pageNumber, pageSize, searchText);

                await Task.WhenAll(countTask, booksTask);

                paginatedBooksResult.TotalCount = await countTask;
                books = await booksTask;
            }
            else
                books = await _bookDataRepository.GetBooksPricesFromWebsites(pageNumber, pageSize, searchText);


            books.ForEach(book => book.LastCrawled = SetLastCrawledString(book.CreatedOn));

            var EanGroups = books.GroupBy(b => b.EAN);

            List<BookWithPriceList> booksWithPriceList = new();
            foreach (var group in EanGroups)
            {
                var book = new BookWithPriceList() { EAN = group.Key, Title = group.First().Title };
                var sitePrices = _mapper.Map<List<WebsitePrice>>(group.ToList());
                book.SitePrices = sitePrices;
                booksWithPriceList.Add(book);
            }
            paginatedBooksResult.BooksWithPrices = booksWithPriceList;
            return paginatedBooksResult;
        }


        public async Task<DownloadableBookData> GetCompleteDownloadableBooksWithPricesList()
        {
            DownloadableBookData paginatedBooksResult = new();
            var totalCount = _bookDataRepository.Books.GroupBy(b => b.EAN).Count();

            var books = await _bookDataRepository.GetBooksPricesFromWebsites(1, totalCount, "");

            paginatedBooksResult.BookDataModel = _mapper.Map<List<BookDataModel>>(books);

            var EanGroups = books.GroupBy(b => b.EAN);
            List<BookWithPriceList> booksWithPriceList = new();
            foreach (var group in EanGroups)
            {
                var book = new BookWithPriceList() { EAN = group.Key, Title = group.First().Title };
                var sitePrices = _mapper.Map<List<WebsitePrice>>(group.ToList());
                book.SitePrices = sitePrices;
                booksWithPriceList.Add(book);
            }
            paginatedBooksResult.BookWithPriceList = booksWithPriceList;
            return paginatedBooksResult;
        }


        public async Task<DownloadableBookData> GetDownloadableBooksDataFromLastHour()
        {
            DownloadableBookData downloadableBooksResult = new();
            var books = await _bookDataRepository.GetBooksPricesFromLastHour();
            books.ForEach(book => book.LastCrawled = SetLastCrawledString(book.CreatedOn));

            downloadableBooksResult.BookDataModel = _mapper.Map<List<BookDataModel>>(books);

            var EanGroups = books.GroupBy(b => b.EAN);
            List<BookWithPriceList> booksWithPriceList = new();
            foreach (var group in EanGroups)
            {
                var book = new BookWithPriceList() { EAN = group.Key, Title = group.First().Title };
                var sitePrices = _mapper.Map<List<WebsitePrice>>(group.ToList());
                book.SitePrices = sitePrices;
                booksWithPriceList.Add(book);
            }
            downloadableBooksResult.BookWithPriceList = booksWithPriceList;
            return downloadableBooksResult;
        }


        public async Task<List<BookWithPriceList>> GetBookPricesFromEan(List<string> EanList)
        {
            var books = await _bookDataRepository.GetBookPricesFromEan(EanList);
            var EanGroups = books.GroupBy(b => b.EAN);
            List<BookWithPriceList> booksWithPriceList = new();
            foreach (var group in EanGroups)
            {
                var book = new BookWithPriceList() { EAN = group.Key, Title = group.First().Title };
                var sitePrices = _mapper.Map<List<WebsitePrice>>(group.ToList());
                book.SitePrices = sitePrices;
                booksWithPriceList.Add(book);
            }
            return booksWithPriceList;
        }

        public async Task<CrawlStatus> GetCrawlStatus()
        {
            return await _bookDataRepository.GetCrawlStatus();
        }

        private static string SetLastCrawledString(DateTime lastCrawled)
        {
            var lastCrawledTime = (DateTime.UtcNow - lastCrawled);
            return lastCrawledTime.TotalDays < 1 ?
                                        lastCrawledTime.TotalHours < 1 ? $"{Convert.ToInt32(lastCrawledTime.TotalMinutes)} min(s) ago" : $"{Convert.ToInt32(lastCrawledTime.TotalHours)} hours(s) ago"
                                        : $"{Convert.ToInt32(lastCrawledTime.TotalDays)} day(s) ago";
        }

       
    }
}
