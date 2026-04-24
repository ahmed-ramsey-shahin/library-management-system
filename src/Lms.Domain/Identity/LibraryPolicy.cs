namespace Lms.Domain.Identity
{
    public record LibraryPolicy (
        int MaxActiveBorrows,
        int MaxUnpaidFines,
        int MaxLateBorrows
    );
}
