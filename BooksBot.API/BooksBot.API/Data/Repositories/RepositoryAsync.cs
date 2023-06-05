using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksBot.API.Data.Repositories
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : class
    {
        private readonly BooksBotContext _dbContext;

        public RepositoryAsync(BooksBotContext dbContext) => _dbContext = dbContext;

        public IQueryable<T> Entities => _dbContext.Set<T>();

        //public async Task<T> AddAsync(T entity)
        //{
        //    await _dbContext.Set<T>().AddAsync(entity);
        //    return entity;
        //}

        public async Task<List<T>> AddRangeAsync(List<T> entities)
        {
            _dbContext.Database.SetCommandTimeout(0);
            await _dbContext.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        //public Task DeleteAsync(T entity)
        //{
        //    _dbContext.Set<T>().Remove(entity);
        //    return Task.CompletedTask;
        //}

        //public Task DeleteRangeAsync(List<T> entities)
        //{
        //    _dbContext.Set<T>().RemoveRange(entities);
        //    return Task.CompletedTask;
        //}

        //public async Task<List<T>> GetAllAsync()
        //{
        //    return await _dbContext
        //        .Set<T>()
        //        .ToListAsync();
        //}

        //public async Task<T> GetByIdAsync(int id)
        //{
        //    return await _dbContext.Set<T>().FindAsync(id);
        //}

        //public Task UpdateAsync(T entity)
        //{
        //    _dbContext.Entry(entity).CurrentValues.SetValues(entity);
        //    return Task.CompletedTask;
        //}

        public Task UpdateRangeAsync(List<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
            return Task.CompletedTask;
        }
    }
}