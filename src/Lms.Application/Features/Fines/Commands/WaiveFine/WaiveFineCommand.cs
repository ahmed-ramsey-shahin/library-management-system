using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Fines.Commands.WaiveFine
{
    public sealed record WaiveFineCommand(Guid BorrowRecordId, Guid FineId) : IRequest<Result<Updated>>;
}
