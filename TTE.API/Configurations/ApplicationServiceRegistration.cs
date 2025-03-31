using TTE.Application.Interfaces;
using TTE.Application.Mapping;
using TTE.Application.Services;
using TTE.Commons.Services;
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
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            return services;
        }
    }
}