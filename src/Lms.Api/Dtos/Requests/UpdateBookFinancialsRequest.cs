namespace Lms.Api.Dtos.Requests
{
    public record UpdateBookFinancialsRequest
    (
        decimal? BorrowPricePerDay,
        decimal? FinePerDay,
        decimal? LostFee,
        decimal? DamageFee
    );
}
