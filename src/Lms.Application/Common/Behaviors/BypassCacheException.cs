namespace Lms.Application.Common.Behaviors
{
    public class BypassCacheException(object errorResponse) : Exception
    {
        public object ErrorResponse { get; } = errorResponse;
    }
}
