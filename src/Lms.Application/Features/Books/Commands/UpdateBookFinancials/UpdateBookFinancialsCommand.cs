using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateBookFinancials
{
    public sealed record UpdateBookFinancialsCommand(
        Guid BookId,
        decimal? BorrowPricePerDay,
        decimal? FinePerDay,
        decimal? LostFee,
        decimal? DamageFee
    ) : IRequest<Result<Updated>>;
}
