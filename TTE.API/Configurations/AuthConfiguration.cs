using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TTE.Commons;

namespace TTE.API.Configurations
{
    public static class AuthConfiguration
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = EnvVariables.AUTH_TOKEN_URL;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = EnvVariables.AUTH_TOKEN_URL,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EnvVariables.JWT_SECRET))
                    };
                });
            return services;
        }
    }
}