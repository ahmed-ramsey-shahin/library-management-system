using System.Reflection;
using FluentValidation;
using Lms.Application.Common.Behaviors;
using Lms.Application.Common.Configurations;
using Lms.Domain.Circulation.Policies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lms.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                configuration.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
                configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
                configuration.AddOpenBehavior(typeof(PerformanceBehavior<,>));
                configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
                configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddScoped<IEnumerable<IBorrowPolicy>>(services =>
            {
                var settings = services.GetRequiredService<IOptionsSnapshot<BorrowSettings>>().Value;
                return [
                    new MaxActiveBorrowsPolicy(settings.MaxActiveBorrows),
                    new MaxLateBorrowsPolicy(settings.MaxLateBorrows),
                    new MaxUnpaidFinesPolicy(settings.MaxUnpaidFines),
                ];
            });
            services.AddScoped<IEnumerable<IRenewalPolicy>>(services =>
            {
                var settings = services.GetRequiredService<IOptionsSnapshot<BorrowSettings>>().Value;
                return [
                    new MaxLateBorrowsPolicy(settings.MaxLateBorrows),
                    new MaxUnpaidFinesPolicy(settings.MaxUnpaidFines),
                    new MaxRenewalCountPolicy(settings.MaxRenewalCount),
                ];
            });
            return services;
        }
    }
}
