using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TTE.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using TTE.Infrastructure.Data;

namespace TTE.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(connectionString, serverVersion);
            });

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;

        }
    }
}
