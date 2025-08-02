using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Project.Domain.Entities;
using Project.Infrastructure.Data.Contexts;

namespace MotorbikeRental.API.Extensions
{
    public static class SecurityExtension
    {
        public static IServiceCollection RegisterSecurityService(this IServiceCollection services, IConfiguration configuration)
        {
            #region Identity Configuration
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            #endregion
            #region JWT configuration
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                byte[] key = Encoding.UTF8.GetBytes(configuration["AppSettings:JwtConfig:Secret"]);
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["AppSettings:JwtConfig:ValidAudience"],
                    ValidIssuer = configuration["AppSettings:JwtConfig:ValidIssuer"],
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion
            return services;
        }
    }
}