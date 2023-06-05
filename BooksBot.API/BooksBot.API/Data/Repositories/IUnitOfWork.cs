using System;
using System.Threading;
using System.Threading.Tasks;

namespace BooksBot.API.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> Commit(CancellationToken cancellationToken);

        Task Commit();

        void Rollback();
    }
}