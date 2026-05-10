using Serilog.Context;

namespace Lms.Api.Infrastructure
{
    public class RequestLogContextMiddleware(RequestDelegate next)
    {
        public Task InvokeAsync(HttpContext context)
        {
            using(LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
            {
                return next(context);
            }
        }
    }
}
