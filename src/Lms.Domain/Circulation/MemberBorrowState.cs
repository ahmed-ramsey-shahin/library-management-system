namespace Lms.Domain.Circulation
{
    public record MemberBorrowState(
        int ActiveBorrows,
        int UnpaidFines,
        int LateBorrows
    );
}
