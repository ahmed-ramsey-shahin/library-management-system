using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Fines.Commands.ChangeFineAmount
{
    public sealed record ChangeFineAmountCommand(
        Guid BorrowRecordId,
        Guid FineId,
        decimal Amount
    ) : IRequest<Result<Updated>>;
}
