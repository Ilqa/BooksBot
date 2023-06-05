using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksBot.API.Data.Repositories
{
    public interface IRepositoryAsync<T> where T : class
    {
        IQueryable<T> Entities { get; }

        //Task<T> GetByIdAsync(int id);

        //Task<List<T>> GetAllAsync();
        //Task<T> AddAsync(T entity);

        Task<List<T>> AddRangeAsync(List<T> entities);

        //Task UpdateAsync(T entity);

        Task UpdateRangeAsync(List<T> entities);

        //Task DeleteAsync(T entity);

        //Task DeleteRangeAsync(List<T> entities);


    }
}