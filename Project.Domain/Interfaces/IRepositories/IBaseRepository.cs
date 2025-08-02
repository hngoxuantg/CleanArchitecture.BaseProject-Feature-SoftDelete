using System.Linq.Expressions;

namespace Project.Domain.Interfaces.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellation = default);

        Task<T?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellation = default);

        Task<T?> GetByIdAsync<Tid>(Tid id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<TResult?> GetQueryAsync<TResult>(CancellationToken cancellation = default,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null);

        Task<(IEnumerable<T>, int totalCount)> GetPagedAsync(Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default);

        Task<T> CreateAsync(T model, CancellationToken cancellation = default);

        Task CreateRange(IEnumerable<T> models, CancellationToken cancellation = default);

        Task<T> UpdateAsync(T model, CancellationToken cancellation = default);

        Task UpdateRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default);

        Task DeleteAsync(T model, CancellationToken cancellation = default);

        Task DeleteRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default);

        Task SaveChangeAsync(CancellationToken cancellation = default);

        Task<bool> IsExistsAsync<TValue>(TValue value, CancellationToken cancellation = default);

        Task<bool> IsExistsForUpdateAsync<Tid, TValue>(Tid id, string key, TValue value, CancellationToken cancellation = default);

        void AddEntity(T entity);

        void AddRangeEntity(IEnumerable<T> entities);

        void UpdateEntity(T entity);

        void DeleteEntity(T entity);

        void DeleteRangeEntity(IEnumerable<T> entities);
    }
}
