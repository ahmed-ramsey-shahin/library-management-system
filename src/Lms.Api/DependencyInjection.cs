using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Lms.Api.Infrastructure;
using Lms.Api.Services;
using Lms.Application.Common.Interfaces;
using Lms.Infrastructure;
using Lms.Infrastructure.Data;
using Lms.Infrastructure.Settings;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Lms.Api
{
    public static class DependencyInjection
    {
        private static IServiceCollection AddOutputCaching(this IServiceCollection services)
        {
            services.AddOutputCache(options =>
            {
                options.SizeLimit = 100 * 1024 * 1024;
                options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromSeconds(60)));
            });
            return services;
        }

        private static IServiceCollection AddAppOpenTelemetry(this IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(res => res.AddService("orderservice"))
                .WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();
                    tracing.AddOtlpExporter();
                }).WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();
                    metrics.AddOtlpExporter().AddPrometheusExporter();
                });
            return services;
        }

        private static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddSlidingWindowLimiter("SlidingWindow", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 100;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.SegmentsPerWindow = 6;
                    limiterOptions.QueueLimit = 10;
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiterOptions.AutoReplenishment = true;
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
            return services;
        }

        private static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(options => options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            });
            return services;
        }

        private static IServiceCollection AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            return services;
        }

        private static IServiceCollection AddExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            return services;
        }

        private static IServiceCollection AddControllerWithJsonConfiguration(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            return services;
        }

        private static IServiceCollection AddValidation(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUser, CurrentUserService>();
            services.AddHttpContextAccessor();
            return services;
        }

        private static IServiceCollection AddConfiguredCors(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>()!;
            services.AddCors(options => options.AddPolicy(
                appSettings.CorsPolicyName,
                policy => policy
                    .WithOrigins(appSettings.AllowedOrigins!)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            ));
            return services;
        }

        public static IApplicationBuilder UseCoreMiddlewars(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseExceptionHandler();
            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseCors(configuration["AppSettings:CorsPolicyName"]!);
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOutputCache();
            app.UseBackgroundJobs();
            return app;
        }

        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddValidation()
                .AddOutputCaching()
                .AddAppOpenTelemetry()
                .AddAppRateLimiting()
                .AddCustomProblemDetails()
                .AddCustomApiVersioning()
                .AddExceptionHandling()
                .AddControllerWithJsonConfiguration()
                .AddIdentityInfrastructure()
                .AddConfiguredCors(configuration);
            return services;
        }

        public static async Task<IApplicationBuilder> SeedDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            if (dbContext is DbContext efDbContext)
            {
                await efDbContext.Database.EnsureCreatedAsync();
                var seedGraph = DatabaseSeeder.GenerateAllMockData(passwordHasher);

                if (!await dbContext.Publishers.AnyAsync())
                {
                    await efDbContext.AddRangeAsync(seedGraph.Publishers);
                    await dbContext.SaveChangesAsync(default);
                }

                if (!await dbContext.Audiences.AnyAsync())
                {
                    await efDbContext.AddRangeAsync(seedGraph.Audiences);
                    await dbContext.SaveChangesAsync(default);
                }

                if (!await dbContext.Authors.AnyAsync())
                {
                    await efDbContext.AddRangeAsync(seedGraph.Authors);
                    await dbContext.SaveChangesAsync(default);
                }

                if (!await dbContext.Genres.AnyAsync())
                {
                    await efDbContext.AddRangeAsync(seedGraph.Genres);
                    await dbContext.SaveChangesAsync(default);
                }

                if (!await dbContext.Categories.AnyAsync())
                {
                    await efDbContext.AddRangeAsync(seedGraph.Categories);
                    await dbContext.SaveChangesAsync(default);
                }

                if (!await dbContext.Themes.AnyAsync())
                {
                    await efDbContext.AddRangeAsync(seedGraph.Themes);
                    await dbContext.SaveChangesAsync(default);
                }

                if (!await dbContext.Keywords.AnyAsync())
                {
                    await efDbContext.AddRangeAsync(seedGraph.Keywords);
                    await dbContext.SaveChangesAsync(default);
                }

                if (!await dbContext.Users.AnyAsync())
                {
                    await efDbContext.AddRangeAsync(seedGraph.Users);
                    await dbContext.SaveChangesAsync(default);
                }

                if (!await dbContext.Books.AnyAsync())
                {
                    await efDbContext.AddRangeAsync(seedGraph.Books);
                    await dbContext.SaveChangesAsync(default);
                }
            }

            return app;
        }
    }
}
