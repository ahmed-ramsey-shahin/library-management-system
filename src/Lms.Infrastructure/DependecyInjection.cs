using Lms.Application.Common.Interfaces;
using Lms.Infrastructure.Data;
using Lms.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Lms.Infrastructure.Services;
using Resend;

namespace Lms.Infrastructure
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            // time provider configuration
            services.AddSingleton(TimeProvider.System);
            // ef core configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                options.AddInterceptors(serviceProvider.GetService<ISaveChangesInterceptor>()!);
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>()!);
            // email service configuration
            services.AddHttpClient<ResendClient>();
            services.Configure<ResendClientOptions>(options => options.ApiToken = configuration["Email:ApiKey"]!);
            services.AddTransient<IResend, ResendClient>();
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}
