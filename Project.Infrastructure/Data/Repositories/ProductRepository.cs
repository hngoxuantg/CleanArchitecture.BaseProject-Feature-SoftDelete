using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories;
using Project.Infrastructure.Data.Contexts;

namespace Project.Infrastructure.Data.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
