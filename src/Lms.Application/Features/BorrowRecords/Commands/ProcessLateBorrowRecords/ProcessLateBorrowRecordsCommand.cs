using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.ProcessLateBorrowRecords
{
    public sealed record ProcessLateBorrowRecordsCommand : IRequest<Result<Updated>>;
}
