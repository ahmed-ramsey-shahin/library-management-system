using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.MarkRecordAsLate
{
    public sealed record MarkRecordAsLateCommand(
        Guid BorrowRecordId
    ) : IRequest<Result<Updated>>;
}
