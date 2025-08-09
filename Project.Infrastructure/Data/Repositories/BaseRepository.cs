using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories;
using Project.Infrastructure.Data.Contexts;
using System.Linq.Expressions;

namespace Project.Infrastructure.Data.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        protected bool IsBaseEntity => typeof(BaseEntity).IsAssignableFrom(typeof(T));

        protected IQueryable<T> ApplyActiveFilter(IQueryable<T> query)
        {
            if (IsBaseEntity)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var isDeletedProperty = Expression.Property(parameter, nameof(BaseEntity.DeleteAt));
                var constant = Expression.Constant(null);
                var notDeleted = Expression.Equal(isDeletedProperty, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(notDeleted, parameter);
                return query.Where(lambda);
            }
            return query;
        }
        #region GetAll methods
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellation = default)
        {
            return await _dbContext.Set<T>()
                .AsNoTracking()
                .ToListAsync(cancellation);
        }
        public async Task<IEnumerable<T>> GetAllActiveAsync(CancellationToken cancellation = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();
            query = ApplyActiveFilter(query);
            return await query.ToListAsync(cancellation);
        }
        #endregion

        #region GetById methods
        public async Task<T?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellation = default)
        {
            return await _dbContext.Set<T>().FindAsync(id, cancellation);
        }
        public async Task<T?> GetActiveByIdAsync<Tid>(Tid id, CancellationToken cancellation = default)
        {
            T? entity = await _dbContext.Set<T>().FindAsync(id, cancellation);
            if (entity != null && IsBaseEntity)
            {
                BaseEntity? baseEntity = entity as BaseEntity;
                return baseEntity?.IsDeleted == true ? null : entity;
            }
            return entity;
        }
        public async Task<T?> GetByIdAsync<Tid>(Tid id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            if (include == null)
                return await _dbContext.Set<T>().FindAsync(id, cancellation);
            string? keyProperty = _dbContext.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties?.FirstOrDefault()?.Name;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, keyProperty);
            var constant = Expression.Constant(id);
            var equality = Expression.Equal(property, constant);
            var lamda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            IQueryable<T> query = _dbContext.Set<T>();

            return await include.Compile()(query).Where(lamda).FirstOrDefaultAsync(cancellation);
        }
        public async Task<T?> GetActiveByIdAsync<Tid>(Tid id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            if (include == null)
                return await GetActiveByIdAsync(id, cancellation);
            string? keyProperty = _dbContext.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties?.FirstOrDefault()?.Name;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, keyProperty);
            var constant = Expression.Constant(id);
            var equality = Expression.Equal(property, constant);
            var lamda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            IQueryable<T> query = _dbContext.Set<T>();
            query = ApplyActiveFilter(query);

            return await include.Compile()(query).Where(lamda).FirstOrDefaultAsync(cancellation);
        }
        #endregion

        #region GetOneUntracked methods
        public async Task<TResult?> GetOneUntrackedAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();
            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            if (selector != null)
                return await query.Select(selector).FirstOrDefaultAsync(cancellation);
            else
                return await query.Cast<TResult>().FirstOrDefaultAsync(cancellation);
        }

        public async Task<TResult?> GetActiveOneUntrackedAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            query = ApplyActiveFilter(query);

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            if (selector != null)
                return await query.Select(selector).FirstOrDefaultAsync(cancellation);
            else
                return await query.Cast<TResult>().FirstOrDefaultAsync(cancellation);
        }

        public async Task<TResult?> GetOneAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            if (selector != null)
                return await query.Select(selector).FirstOrDefaultAsync(cancellation);
            else
                return await query.Cast<TResult>().FirstOrDefaultAsync(cancellation);
        }

        public async Task<TResult?> GetActiveOneAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            query = ApplyActiveFilter(query);

            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            if (selector != null)
                return await query.Select(selector).FirstOrDefaultAsync(cancellation);
            else
                return await query.Cast<TResult>().FirstOrDefaultAsync(cancellation);
        }
        #endregion

        #region GetPaged methods
        public virtual async Task<(IEnumerable<T>, int totalCount)> GetPagedAsync(Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            int count = await query.CountAsync(cancellationToken);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return (await query.ToListAsync(cancellationToken), count);
        }

        public virtual async Task<(IEnumerable<T>, int totalCount)> GetActivePagedAsync(Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            query = ApplyActiveFilter(query);

            if (filter != null)
                query = query.Where(filter);

            int count = await query.CountAsync(cancellationToken);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return (await query.ToListAsync(cancellationToken), count);
        }
        #endregion

        #region CRUD Methods
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
        #endregion

        #region SoftDelete Methods
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
        #endregion


        public async Task SaveChangeAsync(CancellationToken cancellation = default)
        {
            await _dbContext.SaveChangesAsync(cancellation);
        }

        #region Existence Methods
        public async Task<bool> IsExistsAsync<TValue>(TValue value, CancellationToken cancellation = default)
        {
            string? id = _dbContext.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.FirstOrDefault()?.Name;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, id ?? "Id");
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);
            var lamda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return await _dbContext.Set<T>().AnyAsync(lamda, cancellation);
        }
        public async Task<bool> IsExistsForUpdateAsync<Tid, TValue>(Tid id, string key, TValue value, CancellationToken cancellation = default)
        {
            string? idPrimary = _dbContext.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.FirstOrDefault()?.Name;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, key);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);

            var idProperty = Expression.Property(parameter, idPrimary ?? "Id");
            var idEquality = Expression.NotEqual(idProperty, Expression.Constant(id));

            var combinedExpression = Expression.AndAlso(equality, idEquality);
            var lamda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return await _dbContext.Set<T>().AnyAsync(lamda, cancellation);
        }

        #endregion

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
