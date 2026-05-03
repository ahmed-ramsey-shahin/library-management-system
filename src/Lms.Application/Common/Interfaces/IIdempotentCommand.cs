namespace Lms.Application.Common.Interfaces
{
    public interface IIdempotentCommand
    {
        string IdempotencyKey { get; }
    }
}
