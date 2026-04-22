using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation
{
    public static class BorrowRecordErrors
    {
        public static Error IdRequired => Error.Validation("BorrowRecord.Id.Required", "Fine ID is required.");
        public static Error MemberIdRequired => Error.Validation("BorrowRecord.MemberId.Required", "Member ID is required.");
        public static Error BookCopyId => Error.Validation("BorrowRecord.BookCopyId.Required", "Copy ID is required.");
        public static Error DueDateInvalid => Error.Validation("BorrowRecord.DueDate.Invalid", "The due date must be within 30 days.");
        public static Error PickupDeadlineInvalid => Error.Validation("BorrowRecord.PickupDeadline.Invalid", "The pickup deadline must be within 3 days.");
        public static Error FineNotFound => Error.NotFound("BorrowRecord.FineNotFound", "No fine was found with the specified ID.");
        public static Error FineAlreadyExists => Error.Conflict("BorrowRecord.FineAlreadyExists", "This fine already exists.");
        public static Error ResponseInvalid(BorrowRecordStatus status) => Error.Forbidden("BorrowRecord.ResponseInvalid", $"This request canot be accepted because it is already {status}");
        public static Error MemberNotApplicable => Error.Forbidden("BorrowRecord.UserNotApplicable", "This request cannot be accepted because the user is not eligible to borrow additional books.");
    }
}
