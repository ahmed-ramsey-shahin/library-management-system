namespace Lms.Domain.Circulation
{
    public record LibraryPolicy (
        int MaxActiveBorrows,
        int MaxUnpaidFines,
        int MaxLateBorrows
    );
}
