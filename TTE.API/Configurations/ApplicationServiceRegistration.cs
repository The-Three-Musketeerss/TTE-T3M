using TTE.Application.Interfaces;
using TTE.Application.Services;
using TTE.Commons.Services;
using TTE.Infrastructure.Interfaces;
using TTE.Infrastructure.Repositories;

namespace TTE.API.Configurations
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            return services;
        }
    }
}