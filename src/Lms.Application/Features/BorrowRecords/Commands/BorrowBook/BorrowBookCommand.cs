using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.BorrowBook
{
    public sealed record BorrowBookCommand(
        Guid UserId,
        Guid BookId,
        DateOnly DueDate,
        DateOnly PickupDeadline,
        string IdempotencyKey
    ) : IRequest<Result<Guid>>, IIdempotentCommand;

}
