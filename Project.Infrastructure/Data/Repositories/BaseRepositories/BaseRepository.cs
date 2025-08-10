using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using Project.Infrastructure.Data.Contexts;
using System.Linq.Expressions;

namespace Project.Infrastructure.Data.Repositories.BaseRepositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        protected virtual bool IsBaseEntity => typeof(BaseEntity).IsAssignableFrom(typeof(T));

        protected virtual IQueryable<T> ApplyActiveFilter(IQueryable<T> query)
        {
            if (IsBaseEntity)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, nameof(BaseEntity.DeleteAt));
                var constant = Expression.Constant(null);
                var notDelete = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(notDelete, parameter);
                return query.Where(lambda);
            }
            return query;
        }
        
        protected virtual string? GetPrimaryKeyName()
        {
            return _dbContext.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties?.FirstOrDefault()?.Name;
        }
        protected Expression<Func<T, bool>> CreatePropertyExpression<TValue>(string propertyName, TValue value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);
            return Expression.Lambda<Func<T, bool>>(equality, parameter);
        }
    }
}