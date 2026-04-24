using Lms.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(
        ILogger<TRequest> logger,
        IUser user
    ) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = user.Id ?? Guid.Empty;

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Request: {Name} {UserId} {@Request}", requestName, userId, request);
            }

            return await next(cancellationToken);
        }
    }
}
