using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories;
using Project.Infrastructure.Data.Contexts;

namespace Project.Infrastructure.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
