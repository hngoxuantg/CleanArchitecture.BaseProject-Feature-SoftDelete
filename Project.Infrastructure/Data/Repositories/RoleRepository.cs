using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories;
using Project.Infrastructure.Data.Contexts;

namespace Project.Infrastructure.Data.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
