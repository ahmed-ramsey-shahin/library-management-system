using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Fines.Commands.ProcessLateBorrowRecords
{
    public sealed record ProcessLateBorrowRecordsCommand : IRequest<Result<Updated>>;
}
