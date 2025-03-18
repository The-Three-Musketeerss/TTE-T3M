using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TTE.Commons.Constants;

namespace TTE.API.Configurations
{
    public static class AuthConfiguration
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            var issuer = EnvVariables.AUTH_TOKEN_URL;
            var jwtSecret = EnvVariables.JWT_SECRET;
            var key = Encoding.UTF8.GetBytes(jwtSecret);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = issuer;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,


                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = new SymmetricSecurityKey(key),

                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
                options.AddPolicy("ShopperOnly", policy => policy.RequireRole("Shopper"));
                options.AddPolicy("CanAccessDashboard", policy =>
                    policy.RequireRole("Admin", "Employee"));
            });


            return services;
        }
    }
}