using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Lms.Api.Infrastructure;
using Lms.Api.OpenApi.Transformers;
using Microsoft.AspNetCore.RateLimiting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
            }).AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            return services;
        }

        private static IServiceCollection AddApiDocumentation(this IServiceCollection services)
        {
            string[] versions = ["v1"];

            foreach (var version in versions)
            {
                services.AddOpenApi(version, options =>
                {
                    options.AddDocumentTransformer<VersionInfoTransformer>();
                    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                    options.AddOperationTransformer<BearerSecuritySchemeTransformer>();
                });
            }

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
                .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
            return services;
        }

        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services
                .AddValidation()
                .AddOutputCaching()
                .AddAppOpenTelemetry()
                .AddAppRateLimiting()
                .AddCustomProblemDetails()
                .AddCustomApiVersioning()
                .AddExceptionHandling()
                .AddControllerWithJsonConfiguration();
            return services;
        }
    }
}
