using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.ReportLostBook
{
    public sealed record ReportLostBookCommand(
        Guid BorrowRecordId,
        string IdempotencyKey
    ) : IRequest<Result<Updated>>, IIdempotentCommand;
}
