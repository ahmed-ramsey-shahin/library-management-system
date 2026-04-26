using Lms.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Common.Behaviors
{
    public class IdempotencyBehavior<TRequest, TResponse>(
        HybridCache cache,
        ILogger<IdempotencyBehavior<TRequest, TResponse>> logger
    ) : IPipelineBehavior<TRequest, TResponse> where TRequest : IIdempotentCommand
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            return await cache.GetOrCreateAsync(
                $"idem:{typeof(TRequest).Name}:{request.IdempotencyKey}",
                async cancellationToken =>
                {
                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Executing unique command for key {Key}.", request.IdempotencyKey);
                    }

                    return await next(cancellationToken);
                },
                options: new HybridCacheEntryOptions{
                    Expiration = TimeSpan.FromMinutes(10),
                },
                cancellationToken: cancellationToken
            );
        }
    }
}
