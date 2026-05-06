using Lms.Application.Common.Interfaces;
using Lms.Infrastructure.Data;
using Lms.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Lms.Infrastructure.Services;

namespace Lms.Infrastructure
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton(TimeProvider.System);
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                options.AddInterceptors(serviceProvider.GetService<ISaveChangesInterceptor>()!);
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>()!);
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}
