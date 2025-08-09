using System.Linq.Expressions;

namespace Project.Domain.Interfaces.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        #region GetAll methods
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellation = default);

        Task<IEnumerable<T>> GetAllActiveAsync(CancellationToken cancellation = default);
        #endregion

        #region GetById methods
        Task<T?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellation = default);

        Task<T?> GetActiveByIdAsync<Tid>(Tid id, CancellationToken cancellation = default);

        Task<T?> GetByIdAsync<Tid>(Tid id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<T?> GetActiveByIdAsync<Tid>(Tid id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        #endregion

        #region GetOne Methods

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

        #endregion

        #region Paged Methods

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

        #endregion

        Task<T> CreateAsync(T model, CancellationToken cancellation = default);

        Task CreateRange(IEnumerable<T> models, CancellationToken cancellation = default);

        Task<T> UpdateAsync(T model, CancellationToken cancellation = default);

        Task UpdateRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default);

        Task DeleteAsync(T model, CancellationToken cancellation = default);

        Task DeleteRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default);

        Task SoftDeleteAsync(T entity, Guid deleteBy, CancellationToken cancellation = default);

        Task SoftDeleteRangeAsync(IEnumerable<T> entities, Guid deleteBy, CancellationToken cancellation = default);

        Task RestoreAsync(T entity, CancellationToken cancellation = default);

        Task RestoreRangeAsync(IEnumerable<T> entities, CancellationToken cancellation = default);

        Task SaveChangeAsync(CancellationToken cancellation = default);

        Task<bool> IsExistsAsync<TValue>(TValue value, CancellationToken cancellation = default);

        Task<bool> IsExistsForUpdateAsync<Tid, TValue>(Tid id, string key, TValue value, CancellationToken cancellation = default);

        void AddEntity(T entity);

        void AddRangeEntity(IEnumerable<T> entities);

        void UpdateEntity(T entity);

        void UpdateRangeEntity(IEnumerable<T> entities);

        void DeleteEntity(T entity);

        void DeleteRangeEntity(IEnumerable<T> entities);

        void SoftDeleteEntity(T entity, Guid deleteBy);

        void SoftDeleteRangeEntity(IEnumerable<T> entities, Guid deleteBy);

        void RestoreEntity(T entity);

        void RestoreRangeEntity(IEnumerable<T> entities);
    }
}
