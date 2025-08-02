using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories;
using Project.Infrastructure.Data.Contexts;

namespace Project.Infrastructure.Data.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
