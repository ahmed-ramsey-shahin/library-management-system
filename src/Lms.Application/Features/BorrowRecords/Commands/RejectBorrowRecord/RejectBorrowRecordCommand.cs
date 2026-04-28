using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.RejectBorrowRecord
{
    public sealed record RejectBorrowRecordCommand(
        Guid BorrowRecordId
    ) : IRequest<Result<Updated>>;
}
