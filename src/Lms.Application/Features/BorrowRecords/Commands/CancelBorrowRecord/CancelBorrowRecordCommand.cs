using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.CancelBorrowRecord
{
    public sealed record CancelBorrowRecordCommand(
        Guid BorrowRecordId
    ) : IRequest<Result<Updated>>;
}
