using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Common.Behaviors
{
    public class CachingBehavior<TRequest, TResponse>(
        HybridCache cache,
        ILogger<CachingBehavior<TRequest, TResponse>> logger
    ) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not ICachedQuery cachedQuery)
            {
                return await next(cancellationToken);
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Checking cache for {RequestName}", typeof(TRequest).Name);
            }

            return await cache.GetOrCreateAsync(
                key: cachedQuery.CacheKey,
                factory: async cancellationToken =>
                {
                    var result = await next(cancellationToken);
                    if (result is IResult r && r.IsSuccess)
                    {
                        return result;
                    }
                    return default!;
                },
                options: new HybridCacheEntryOptions
                {
                    Expiration = cachedQuery.Expiration,
                },
                tags: cachedQuery.Tags,
                cancellationToken: cancellationToken
            );
        }
    }
}
