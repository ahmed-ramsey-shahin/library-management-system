using MediatR;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Common.Behaviors
{
    public class UnhandledExceptionBehavior<TRequest, TResponse>(ILogger<TRequest> loggger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next(cancellationToken);
            }
            catch(Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                if (loggger.IsEnabled(LogLevel.Error))
                {
                    loggger.LogError(ex, "Request: Unhandled exception for request {Name} {@Request}", requestName, request);
                }
                throw;
            }
        }
    }
}
