using Lms.Domain.Common.Results;

namespace Lms.Domain.Identity
{
    public static class UserErrors
    {
        // --- Validation Errors ---
        public static Error IdRequired => Error.Validation("User.IdRequired", "User ID is required.");
        public static Error FirstNameRequired => Error.Validation("User.FirstNameRequired", "First name is required.");
        public static Error LastNameRequired => Error.Validation("User.LastNameRequired", "Last name is required.");
        public static Error PhoneNumberRequired => Error.Validation("User.PhoneNumberRequired", "Phone number is required.");
        public static Error AddressRequired => Error.Validation("User.AddressRequired", "Address is required.");
        public static Error LibraryCardNumberRequired => Error.Validation("User.LibraryCardNumberRequired", "Library card number is required.");
        public static Error PasswordRequired => Error.Validation("User.PasswordRequired", "Password is required.");
        public static Error EmailRequired => Error.Validation("User.EmailRequired", "Email address is required.");

        // --- Conflict Errors ---
        public static Error RoleInvalid => Error.Conflict("User.RoleInvalid", "A librarian or admin cannot become a member. The account must be deleted instead.");
        public static Error MemberDeletionFailed => Error.Conflict("User.MemberDeletionFailed", "A member cannot be deleted until all fines are paid and all books are returned.");

        // --- Forbidden Errors ---
        public static Error NotLibrarian => Error.Forbidden("User.NotLibrarian", "Only librarians can have categories assigned to them.");
        public static Error UserSuspended => Error.Forbidden("User.Suspended", "This operation cannot be performed because the user is suspended.");
    }
}
