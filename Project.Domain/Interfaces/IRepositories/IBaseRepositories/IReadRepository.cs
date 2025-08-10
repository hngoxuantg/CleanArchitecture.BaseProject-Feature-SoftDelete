using System.Linq.Expressions;

namespace Project.Domain.Interfaces.IRepositories.IBaseRepositories
{
    public interface IReadRepository<T> : IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellation = default);

        Task<IEnumerable<T>> GetAllActiveAsync(CancellationToken cancellation = default);

        Task<T?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellation = default);

        Task<T?> GetActiveByIdAsync<Tid>(Tid id, CancellationToken cancellation = default);

        Task<T?> GetByIdAsync<Tid>(Tid id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<T?> GetActiveByIdAsync<Tid>(Tid id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<TResult?> GetOneUntrackedAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<TResult?> GetActiveOneUntrackedAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<TResult?> GetOneAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<TResult?> GetActiveOneAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<bool> IsExistsAsync<TValue>(TValue value, CancellationToken cancellation = default);

        Task<bool> IsExistsForUpdateAsync<Tid, TValue>(Tid id, string key, TValue value, CancellationToken cancellation = default);

        Task<(IEnumerable<T>, int totalCount)> GetPagedAsync(Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default);

        Task<(IEnumerable<T>, int totalCount)> GetActivePagedAsync(Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default);
    }
}
