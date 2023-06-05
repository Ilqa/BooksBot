using BooksBot.API.Data.Entities;
using BooksBot.API.Extensions;
using GenPsych.Application.CacheKeys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using System;
using BooksBot.API.Models;

namespace BooksBot.API.Data.Repositories
{
    public class BookDataRepository : IBookDataRepository
    {
        private readonly IRepositoryAsync<BookData> _repository;
        private readonly IDistributedCache _distributedCache;
        private readonly BooksBotContext _dbContext;

        public BookDataRepository(IRepositoryAsync<BookData> repository, BooksBotContext bookDataContext, IDistributedCache distributedCache)
        {
            _repository = repository;
            _distributedCache = distributedCache;
            _dbContext = bookDataContext;
        }

        public IQueryable<BookData> Books => _repository.Entities;

        public async Task AddRangeAsync(List<BookData> entities)
        {
            await _repository.AddRangeAsync(entities);
            await _distributedCache.RemoveAsync(CacheKeys.BooksListKey);
        }

        public async Task<List<BookData>> GetBooksPricesFromWebsites(int pageNumber, int pageSize, string searchText)
        {
            _dbContext.Database.SetCommandTimeout(0);
            var booksQueryable = _dbContext.BookData.FromSqlInterpolated($"EXECUTE dbo.sp_GetBooksWithPrices {pageNumber}, {pageSize}, {searchText}");
            booksQueryable.Include(b => b.CrawlSource).ThenInclude(cs => cs.SourceWebsite);
            var books = booksQueryable.ToList();
            return books;

        }

        public async Task<int> GetTotalBooksCount(string searchText)
        {
            _dbContext.Database.SetCommandTimeout(0);
            var countParam = new SqlParameter("Count", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var searchTextParam = new SqlParameter("SearchText", searchText);

             _dbContext.Database.ExecuteSqlRaw($"dbo.sp_GetTotalBooksCount @SearchText,  @Count OUT", parameters: new object[] {searchTextParam, countParam, });

            var bookcount = Convert.ToInt32(countParam.Value);
            return bookcount;
        }

        public async Task<List<BookData>> GetAllBooksPricesFromWebsites()
        {
            _dbContext.Database.SetCommandTimeout(0);
            var booksQueryable = _dbContext.BookData.FromSqlInterpolated($"EXECUTE dbo.sp_GetAllBooksWithPrices");
            booksQueryable.Include(b => b.CrawlSource).ThenInclude(cs => cs.SourceWebsite);
            var books = booksQueryable.ToList();
            return books;

        }


        public async Task<List<BookData>> GetBooksPricesFromLastHour()
        {
            _dbContext.Database.SetCommandTimeout(0);
            var booksQueryable = _dbContext.BookData.FromSqlInterpolated($"EXECUTE dbo.sp_GetBooksWithPricesFromLastHour");
            booksQueryable.Include(b => b.CrawlSource).ThenInclude(cs => cs.SourceWebsite);
            return booksQueryable.ToList();
        }

        public async Task<string> ArchiveBookData()
        {
            try {
                _dbContext.Database.SetCommandTimeout(0); 
                _dbContext.Database.ExecuteSqlInterpolated($"EXECUTE dbo.sp_ArchiveBookData"); }
            catch { return "Book Data Archive Failed"; }

            return "Book Data Archived Successfully";

        }

        public async Task<List<BookData>> GetBookPricesFromEan(List<string> eanList)
        {
           List<BookData> books = new();

            if (eanList.Any())
            {
                _dbContext.Database.SetCommandTimeout(0);
                var eanListString = string.Join(",", eanList);
                var booksQueryable = _dbContext.BookData.FromSqlInterpolated($"EXECUTE dbo.sp_GetBookPricesFromEan {eanListString}");
                books = booksQueryable.ToList();    
            }
            return books;
        }

        public async Task<CrawlStatus> GetCrawlStatus()
        {
            _dbContext.Database.SetCommandTimeout(0);
            var successfulCrawls = new SqlParameter("successfulCrawls", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var failedCrawls = new SqlParameter("failedCrawls", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var distinctEans = new SqlParameter("distinctEans", SqlDbType.Int) { Direction = ParameterDirection.Output };

            _dbContext.Database.ExecuteSqlRaw($"dbo.sp_GetCrawlStatus @SuccessfulCrawls out, @FailedCrawls out, @DistinctEans out", parameters: new object[] { successfulCrawls, failedCrawls, distinctEans });

            CrawlStatus crawlstatus = new() { SuccessfulCrawls = Convert.ToInt32(successfulCrawls.Value), FailedCrawls = Convert.ToInt32(failedCrawls.Value), DistinctEans = Convert.ToInt32(distinctEans.Value) };
           
            return crawlstatus;
        }
    }
}
