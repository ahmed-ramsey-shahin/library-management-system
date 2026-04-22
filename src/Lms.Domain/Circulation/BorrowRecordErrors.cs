using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation
{
    public static class BorrowRecordErrors
    {
        public static Error IdRequired => Error.Validation("BorrowRecord.Id.Required", "Fine ID is required.");
        public static Error MemberIdRequired => Error.Validation("BorrowRecord.MemberId.Required", "Member ID is required.");
        public static Error BookCopyId => Error.Validation("BorrowRecord.BookCopyId.Required", "Copy ID is required.");
        public static Error DueDateInvalid => Error.Validation("Fine.DueDate.Invalid", "The due date must be within 30 days.");
        public static Error PickupDeadlineInvalid => Error.Validation("Fine.PickupDeadline.Invalid", "The pickup deadline must be within 3 days.");
    }
}
