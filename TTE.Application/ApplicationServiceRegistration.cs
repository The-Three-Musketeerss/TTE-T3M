using Microsoft.Extensions.DependencyInjection;
using TTE.Application.Interfaces;
using TTE.Application.Services;

namespace TTE.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            return services;
        }
    }
}