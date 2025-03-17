using Microsoft.EntityFrameworkCore;
using TTE.Commons;
using TTE.Infrastructure.Data;

namespace TTE.API.Configurations
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = EnvVariables.DB_CONNECTION_STRING;
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(connectionString, serverVersion);
            });

            return services;

        }
    }
}
