using Lms.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Common.Behaviors
{
    public class IdempotencyBehavior<TRequest, TResponse>(
        IMemoryCache cache,
        IUser CurrentUser,
        ILogger<IdempotencyBehavior<TRequest, TResponse>> logger
    ) : IPipelineBehavior<TRequest, TResponse> where TRequest : IIdempotentCommand
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheKey = $"idem:{CurrentUser.Id}:{typeof(TRequest).Name}:{request.IdempotencyKey}";

            if (cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Returning cached idempotent result for key {Key}.", request.IdempotencyKey);
                }

                return cachedResponse!;
            }

            var response = await next(cancellationToken);
            cache.Set(cacheKey, response, TimeSpan.FromMinutes(2));
            return response;
        }
    }
}
