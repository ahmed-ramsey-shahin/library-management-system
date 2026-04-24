namespace Lms.Domain.Identity
{
    public record MemberBorrowState(
        int ActiveBorrows,
        int UnpaidFines,
        int LateBorrows
    );
}
