using BooksBot.API.Constants;
using BooksBot.API.Data.Entities;
using BooksBot.API.Models;
using Microsoft.EntityFrameworkCore;
using StoredProcedureEFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksBot.API.Data.Repositories
{
    public class CrawlSourceRepository : ICrawlSourceRepository
    {
        private readonly IRepositoryAsync<CrawlSource> _repository;
        private readonly BooksBotContext _dbContext;

        public CrawlSourceRepository(IRepositoryAsync<CrawlSource> repository, BooksBotContext bookDataContext) 
        {
            _repository = repository;
            _dbContext = bookDataContext;
        }

        public IQueryable<CrawlSource> CrawlSources => _repository.Entities;

        //public Task<List<CrawlSource>> LongRunningCrawlProcess =>  _repository.Entities.Where(cs => cs.Status == StringConstants.InProgress && cs.CrawlStartTime <= DateTime.UtcNow.AddHours(-1))
        //                                                                         .ToList();

        public async Task UpdateRangeAsync(List<CrawlSource> crawlSources)
        {
            await _repository.UpdateRangeAsync(crawlSources);
        }

        public async Task AddRangeAsync(List<CrawlSource> entities) => await _repository.AddRangeAsync(entities);

        public async Task<List<UrlStatus>> GetUrlsStatusCrawledIn24Hours()
        {
            _dbContext.Database.SetCommandTimeout(0);
            //return await _repository.Entities.Where(cs => cs.ShouldCrawl && cs.CrawlStartTime > DateTime.UtcNow.AddHours(-24)).ToListAsync();
            //return await _dbContext.CrawlSources.Where(cs => cs.ShouldCrawl && cs.CrawlStartTime > DateTime.UtcNow.AddHours(-24))
            //                                                   .Select(p => new UrlStatus() { Url = p.Url, Status = p.Status, CrawlStartTime = p.CrawlStartTime.Value })
            //                                                   .ToListAsync();



            //var query = _dbContext.FromSql("EXECUTE dbo.ListUserClaims);
            //_dbContext.LoadStoredProc("usp_GetContactDetails")

                var urlsStatuses = new List<UrlStatus>();

            _dbContext.LoadStoredProc("sp_GetScrapedUrlsFromLast24Hours")
           .Exec(r =>
           {
               urlsStatuses.AddRange(r.ToList<UrlStatus>());
           });

            return urlsStatuses;




        }
    }
}
