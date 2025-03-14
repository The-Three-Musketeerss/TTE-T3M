﻿using TTE.Application.Interfaces;
using TTE.Application.Services;
using TTE.Infrastructure.Repositories;

namespace TTE.API.Configurations
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }
    }
}