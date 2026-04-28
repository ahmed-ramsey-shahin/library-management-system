using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.AcceptBorrowRecord
{
    public sealed record AcceptBorrowRecordCommand(
        Guid BorrowRecordId
    ) : IRequest<Result<Updated>>;
}
