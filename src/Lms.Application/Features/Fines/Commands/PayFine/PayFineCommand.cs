using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Fines.Commands.PayFine
{
    public sealed record PayFineCommand(
        Guid BorrowRecordId,
        Guid FineId
    ) : IRequest<Result<Updated>>;
}
