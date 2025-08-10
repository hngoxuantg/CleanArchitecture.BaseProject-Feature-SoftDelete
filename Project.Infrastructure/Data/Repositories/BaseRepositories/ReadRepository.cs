using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using Project.Infrastructure.Data.Contexts;
using System.Linq.Expressions;

namespace Project.Infrastructure.Data.Repositories.BaseRepositories
{
    public class ReadRepository<T> : BaseRepository<T>, IReadRepository<T> where T : class
    {
        public ReadRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

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
            string? keyProperty = GetPrimaryKeyName();

            var lamda = CreatePropertyExpression(keyProperty, id);

            IQueryable<T> query = _dbContext.Set<T>();

            return await include.Compile()(query).Where(lamda).FirstOrDefaultAsync(cancellation);
        }

        public async Task<T?> GetActiveByIdAsync<Tid>(Tid id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            if (include == null)
                return await GetActiveByIdAsync(id, cancellation);
            string? keyProperty = GetPrimaryKeyName();

            var lamda = CreatePropertyExpression(keyProperty, id);

            IQueryable<T> query = _dbContext.Set<T>();
            query = ApplyActiveFilter(query);

            return await include.Compile()(query).Where(lamda).FirstOrDefaultAsync(cancellation);
        }

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
    }
}
