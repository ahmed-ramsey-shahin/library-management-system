using Lms.Domain.Common.Results;

namespace Lms.Domain.Identity
{
    public static class UserErrors
    {
        public static Error IdRequired => Error.Validation("User.Id.Required", "User id is required.");
        public static Error EmailRequired => Error.Validation("User.Email.Required", "User email is required.");
        public static Error RoleInvalid => Error.Conflict("User.Role.Invalid", "A librarian or admin cannot become a member. Please delete the account instead.");
        public static Error MemberDeletionFailed => Error.Failure("User.Delete.Failed", "A member cannot be deleted unless all fines are paid and all books are returned.");
    }
}
