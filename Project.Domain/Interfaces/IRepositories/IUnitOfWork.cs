using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Domain.Interfaces.IRepositories
{
    public interface IUnitOfWork
    {
        IReadRepository<T> GetReadRepository<T>() where T : class;
        IWriteRepository<T> GetWriteRepository<T>() where T : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        void Dispose();
    }
}
