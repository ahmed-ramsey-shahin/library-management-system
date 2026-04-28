using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.RenewBook
{
    public sealed record RenewBookCommand(
        Guid BorrowRecordId,
        DateOnly DueDate,
        Guid IdempotencyKey
    ) : IIdempotentCommand, IRequest<Result<Updated>>;

}
