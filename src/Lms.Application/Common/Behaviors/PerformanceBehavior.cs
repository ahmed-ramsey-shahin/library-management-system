using System.Diagnostics;
using Lms.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse>(ILogger<TRequest> logger, IUser user) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly Stopwatch _timer = new();

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();
            var response = await next(cancellationToken);
            _timer.Stop();
            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = user.Id ?? Guid.Empty;
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Long running request: {Name} ({ElapsedMilliseconds} milliseconds) {UserId} {@Request}", requestName, elapsedMilliseconds, userId, request);
                }
            }
            return response;
        }
    }
}
