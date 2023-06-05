using BooksBot.API.Configurations;
using BooksBot.API.Constants;
using BooksBot.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BooksBot.API.Constants.Enums;

namespace BooksBot.API.Data.Repositories
{
    public class ShouldCrawlEanRepository : IShouldCrawlEanRepository
    {
        private readonly IRepositoryAsync<ShouldCrawlEan> _repository;
        private readonly AppConfiguration _appConfig;

        public ShouldCrawlEanRepository(IRepositoryAsync<ShouldCrawlEan> repository, AppConfiguration appConfig)
        {
            _repository = repository;
            _appConfig = appConfig;
        }
        public IQueryable<ShouldCrawlEan> ShouldCrawlEans=> _repository.Entities;

        public async Task<List<ShouldCrawlEan>> GetShouldCrawlEans() => await _repository.Entities.Where(s => !s.CrawlSourceSearched).Take(_appConfig.EanURLCount).ToListAsync();

        public Task<List<ShouldCrawlEan>> GetShouldCrawlEansByShortName(SourceWebSiteShortNameEnum sourceWebSiteShortNameEnum)
        {
            return _repository.Entities.Where(s => !s.CrawlSourceSearched && s.SourceWebSiteShortNameEnum == sourceWebSiteShortNameEnum).Take(_appConfig.EanURLCount).ToListAsync();
        }

        //SET COUNT FROM APP CONFIG
        public async Task UpdateRangeShouldCrawlEan(List<ShouldCrawlEan> crawlEans) => await _repository.UpdateRangeAsync(crawlEans);
    }
}
