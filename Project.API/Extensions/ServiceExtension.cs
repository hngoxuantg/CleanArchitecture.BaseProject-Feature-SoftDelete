using Project.Domain.Interfaces.IRepositories;
using Project.Infrastructure.Data.Repositories;

namespace Project.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            RegisterServices(services);
            RegisterRepositories(services);
            return services;
        }
        public static IServiceCollection RegisterServices(IServiceCollection services)
        {
            return services;
        }
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            return services;
        }
    }
}
