namespace Project.Domain.Interfaces.IRepositories.IBaseRepositories
{
    public interface IWriteRepository<T> : IBaseRepository<T> where T : class
    {
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
