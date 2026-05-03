using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Fines.Commands.IssueFine
{
    public sealed record IssueFineCommand(
        Guid BorrowRecordId,
        decimal Amount,
        string Description,
        DateTimeOffset FineDate,
        string IdempotencyKey
    ) : IRequest<Result<Guid>>, IIdempotentCommand;
}
