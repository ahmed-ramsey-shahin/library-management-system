namespace Lms.Application.Common.Interfaces
{
    public interface IIdempotentCommand
    {
        Guid IdempotencyKey { get; }
    }
}
