namespace Lms.Domain.Circulation.Policies
{
    public sealed record BorrowStats(
        int ActiveBorrows,
        int LateBorrows,
        int UnpaidFines,
        int RenewalCount
    );
}
