using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using Project.Infrastructure.Data.Contexts;

namespace Project.Infrastructure.Data.Repositories.BaseRepositories
{
    public class WriteRepository<T> : BaseRepository<T>, IWriteRepository<T> where T : class
    {
        public WriteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public virtual async Task<T> CreateAsync(T model, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().Add(model);
            await _dbContext.SaveChangesAsync(cancellation);
            return model;
        }

        public virtual async Task CreateRange(IEnumerable<T> models, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().AddRange(models);
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public async Task<T> UpdateAsync(T model, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().Update(model);
            await _dbContext.SaveChangesAsync(cancellation);
            return model;
        }

        public async Task UpdateRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().UpdateRange(models);
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public async Task DeleteAsync(T model, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().Remove(model);
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public async Task DeleteRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().RemoveRange(models);
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public async Task SoftDeleteAsync(T entity, Guid deleteBy, CancellationToken cancellation = default)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.MarkAsDeleted(deleteBy);
                _dbContext.Set<T>().Update(entity);
                await _dbContext.SaveChangesAsync(cancellation);
            }
            else
            {
                await DeleteAsync(entity, cancellation);
            }
        }

        public async Task SoftDeleteRangeAsync(IEnumerable<T> entities, Guid deleteBy, CancellationToken cancellation = default)
        {
            foreach (var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.MarkAsDeleted(deleteBy);
                    _dbContext.Set<T>().Update(entity);
                }
                else
                {
                    _dbContext.Set<T>().Remove(entity);
                }
            }
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public async Task RestoreAsync(T entity, CancellationToken cancellation = default)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.RestoreFromDeleted();
                _dbContext.Set<T>().Update(entity);
            }
            else
            {
                throw new InvalidOperationException("Entity does not support restoration.");
            }
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public async Task RestoreRangeAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
        {
            foreach (var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.RestoreFromDeleted();
                    _dbContext.Set<T>().Update(entity);
                }
                else
                {
                    throw new InvalidOperationException("Entity does not support restoration.");
                }
            }
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public virtual void AddEntity(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public virtual void AddRangeEntity(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
        }

        public virtual void UpdateEntity(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public virtual void UpdateRangeEntity(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
        }

        public virtual void DeleteEntity(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public virtual void DeleteRangeEntity(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public virtual void SoftDeleteEntity(T entity, Guid deleteBy)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.MarkAsDeleted(deleteBy);
                _dbContext.Set<T>().Update(entity);
            }
            else
            {
                DeleteEntity(entity);
            }
        }

        public virtual void SoftDeleteRangeEntity(IEnumerable<T> entities, Guid deleteBy)
        {
            foreach (var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.MarkAsDeleted(deleteBy);
                    _dbContext.Set<T>().Update(entity);
                }
                else
                {
                    DeleteEntity(entity);
                }
            }
        }

        public virtual void RestoreEntity(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.RestoreFromDeleted();
                _dbContext.Set<T>().Update(entity);
            }
            else
            {
                throw new InvalidOperationException("Entity does not support restoration.");
            }
        }

        public virtual void RestoreRangeEntity(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.RestoreFromDeleted();
                    _dbContext.Set<T>().Update(entity);
                }
                else
                {
                    throw new InvalidOperationException("Entity does not support restoration.");
                }
            }
        }
    }
}
