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
        public static Error ReturnInvalid(BorrowRecordStatus status) => Error.Forbidden("BorrowRecord.ReturnInvalid", $"This copy cannot be returned because its already {status}");
        public static Error MemberNotApplicable => Error.Forbidden("BorrowRecord.UserNotApplicable", "This request cannot be accepted because the user is not eligible to borrow additional books.");
        public static Error RenewInvalid(BorrowRecordStatus status) => Error.Forbidden("BorrowRecord.RenewInvalid", $"This record cannot be renewed because its already {status}");
        public static Error CancellationInvalid(BorrowRecordStatus status) => Error.Forbidden("BorrowRecord.CancellationInvalid", $"This record cannot be canceled because its already {status}");
        public static Error MaxRenewalCountExceeded => Error.Forbidden("BorrowRecord.MaxRenewalCountExceeded", "This operation cannot be done because the maximum number of renewals is exceeded.");
        public static Error RejectInvalid(BorrowRecordStatus status) => Error.Forbidden("BorrowRecord.RejectInvalid", $"This request canot be rejected because it is already {status}");
        public static Error PayFineInvalid => Error.Forbidden("BorrowRecord.PayFineInvalid", "Cannot pay fine for this record.");
        public static Error CannotMarkAsLate => Error.Forbidden("BorrowRecord.CannotMarkAsLate", "Cannot mark this record as late.");
        public static Error DueDateLessThanWeek => Error.Forbidden("BorrowRecord.DueDate.DueDateLessThanWeek", "Due date must be more than a week.");
        public static Error PickupDeadlineLessThanDay => Error.Forbidden("BorrowRecord.PickupDeadline.PickupDeadlineLessThanDay", "Pickup deadline can no be less than a day.");
        public static Error DailyFineAlreadyAssessed => Error.Forbidden("BorrowRecord.Fines.DailyFineAlreadyAsseseed", "The daily fine for this borrow record is already assessed.");
        public static Error CannotMarkAsLost => Error.Forbidden("BorrowRecord.CannotMarkAsLost", "Cannot mark this record as lost.");
        public static Error AlreadyPickedup => Error.Forbidden("BorrowRecord.AlreadyPickedup", "This borrow record is already pickedup.");
    }
}
