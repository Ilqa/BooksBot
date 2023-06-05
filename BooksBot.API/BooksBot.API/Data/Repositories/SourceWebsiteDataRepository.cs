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
using BooksBot.API.Constants;

namespace BooksBot.API.Data.Repositories
{
    public class SourceWebsiteDataRepository : ISourceWebsiteDataRepository
    {
        private readonly IRepositoryAsync<SourceWebsite> _repository;
        private readonly BooksBotContext _dbContext;

        public SourceWebsiteDataRepository(IRepositoryAsync<SourceWebsite> repository, BooksBotContext bookDataContext, IDistributedCache distributedCache)
        {
            _repository = repository;
            _dbContext = bookDataContext;
        }

        public IQueryable<SourceWebsite> SourceWebsite => _repository.Entities;

        public async Task AddRangeAsync(List<SourceWebsite> entities)
        {
            await _repository.AddRangeAsync(entities);
        }

        public async Task<SourceWebsite> GetSourceWebsiteByShortName(Enums.SourceWebSiteShortNameEnum sourceWebSiteShortNameEnum)
        {
            var SourceWebsite = await _repository.Entities.Where(a=>a.SourceWebSiteShortNameEnum == sourceWebSiteShortNameEnum).FirstOrDefaultAsync();
            return SourceWebsite;
        }
    }
}
