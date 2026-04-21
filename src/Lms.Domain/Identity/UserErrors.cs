using Lms.Domain.Common.Results;

namespace Lms.Domain.Identity
{
    public static class UserErrors
    {
        public static Error IdRequired => Error.Validation("User.Id.Required", "User id is required.");
        public static Error FirstNameRequired => Error.Validation("User.FirstName.Required", "User first name is required.");
        public static Error LastNameRequired => Error.Validation("User.LastName.Required", "User last name is required.");
        public static Error PhoneNumberRequired => Error.Validation("User.PhoneNumber.Required", "User phone number is required.");
        public static Error AddressRequired => Error.Validation("User.Address.Required", "User address is required.");
        public static Error LibraryCardNumberRequired => Error.Validation("User.LibraryCardNumberRequired.Required", "Library card number is required.");
        public static Error PasswordRequired => Error.Validation("User.Password.Required", "User password is required.");
        public static Error SaltRequired => Error.Validation("User.Salt.Required", "User salt is required.");
        public static Error EmailRequired => Error.Validation("User.Email.Required", "User email is required.");
        public static Error RoleInvalid => Error.Conflict("User.Role.Invalid", "A librarian or admin cannot become a member. Please delete the account instead.");
        public static Error MemberDeletionFailed => Error.Failure("User.Delete.Failed", "A member cannot be deleted unless all fines are paid and all books are returned.");
        public static Error NotLibrarian => Error.Forbidden("User.AddCategory.NotLibrarian", "Only librarian users have categories assigned to them.");
        public static Error NotMember => Error.Forbidden("User.NotMember", "This operation is only allowed for members.");
        public static Error MaxActiveBorrowsReached(int maxActiveBorrows) => Error.Forbidden("User.MaxActiveBorrowsReached", $"A member cannot have more than {maxActiveBorrows} active borrows.");
        public static Error MaxUnpaidFines(int maxUnpaidFines) => Error.Forbidden("User.MaxUnpaidFines", $"A member cannot borrow a book if the number of unpaid fines exceeds {maxUnpaidFines}.");
        public static Error MaxLateBorrows(int maxLateBorrows) => Error.Forbidden("User.MaxLateBorrows", $"A member cannot borrow a book if the number of currently overdue borrows exceeds {maxLateBorrows}.");
        public static Error UserSuspended => Error.Forbidden("User.Suspended", "This operation cannot be performed because this user is suspended.");
        public static Error CategoryAlreadyAssigned => Error.Conflict("User.CategoryAlreadyAssigned", "This category is already assigned to this librarian.");
        public static Error CategoryNotAssigned => Error.Conflict("User.CategoryNotAssigned", "This category is not assigned to this librarian.");
    }
}
