using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.MarkOverdueBorrowRecords
{
    public sealed record MarkOverdueBorrowRecordsCommand : IRequest<Result<Updated>>;
}
