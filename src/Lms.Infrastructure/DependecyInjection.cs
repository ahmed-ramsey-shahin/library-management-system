using Lms.Application.Common.Interfaces;
using Lms.Infrastructure.Data;
using Lms.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Lms.Infrastructure.Services;
using Resend;
using Hangfire;
using Lms.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Lms.Domain.Identity;
using Microsoft.Extensions.Caching.Hybrid;

namespace Lms.Infrastructure
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            // time provider configuration
            services.AddSingleton(TimeProvider.System);
            // ef core configuration
            var sqlServerConnectionString = configuration.GetConnectionString("SqlServer");
            var redisConnectionString = configuration.GetConnectionString("Redis");
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                options.AddInterceptors(serviceProvider.GetService<ISaveChangesInterceptor>()!);
                options.UseSqlServer(sqlServerConnectionString);
            });
            services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>()!);
            // email service configuration
            services.AddHttpClient<ResendClient>();
            services.Configure<ResendClientOptions>(options => options.ApiToken = configuration["Email:ApiKey"]!);
            services.AddTransient<IResend, ResendClient>();
            services.AddScoped<IEmailService, EmailService>();
            // hangfire service configuration
            services.AddHangfire(config => config.UseSqlServerStorage(sqlServerConnectionString));
            services.AddHangfireServer();
            // interfaces configuration
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<IPasswordGenerator, PasswordGenerator>();
            // authnetication configuration
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)),
                };
            });
            services.AddAuthorizationBuilder()
                .AddPolicy("Admin", policy => policy.RequireRole(nameof(Role.Admin)))
                .AddPolicy("Librarian", policy => policy.RequireRole(nameof(Role.Librarian)))
                .AddPolicy("Member", policy => policy.RequireRole(nameof(Role.Member)));
            // Caching configuration
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "Lms_";
            });
            services.AddHybridCache(options =>
            {
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(10),
                    LocalCacheExpiration = TimeSpan.FromMinutes(2)
                };
            });
            return services;
        }
    }
}
