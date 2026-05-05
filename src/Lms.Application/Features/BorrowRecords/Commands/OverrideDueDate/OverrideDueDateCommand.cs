using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.OverrideDueDate
{
    public sealed record OverrideDueDateCommand(Guid BorrowRecordId, DateOnly DueDate) : IRequest<Result<Updated>>;
}
