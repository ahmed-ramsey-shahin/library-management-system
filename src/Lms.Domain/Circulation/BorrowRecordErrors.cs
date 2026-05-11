using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation
{
    public static class BorrowRecordErrors
    {
        // --- Validation Errors ---
        public static Error IdRequired => Error.Validation("BorrowRecord.IdRequired", "Borrow record ID is required.");
        public static Error MemberIdRequired => Error.Validation("BorrowRecord.MemberIdRequired", "Member ID is required.");
        public static Error BookCopyIdRequired => Error.Validation("BorrowRecord.BookCopyIdRequired", "Copy ID is required.");
        public static Error DueDateInvalid => Error.Validation("BorrowRecord.DueDateInvalid", "The due date must be within 30 days.");
        public static Error PickupDeadlineInvalid => Error.Validation("BorrowRecord.PickupDeadlineInvalid", "The pickup deadline must be within 3 days.");
        public static Error DueDateLessThanWeek => Error.Forbidden("BorrowRecord.DueDateTooShort", "Due date must be at least one week from today.");
        public static Error PickupDeadlineLessThanDay => Error.Forbidden("BorrowRecord.PickupDeadlineTooShort", "Pickup deadline cannot be less than one day.");

        // --- NotFound Errors ---
        public static Error FineNotFound => Error.NotFound("BorrowRecord.FineNotFound", "No fine was found with the specified ID.");

        // --- Conflict Errors ---
        public static Error FineAlreadyExists => Error.Conflict("BorrowRecord.FineAlreadyExists", "A fine already exists for this borrow record.");
        public static Error DailyFineAlreadyAssessed => Error.Conflict("BorrowRecord.DailyFineAlreadyAssessed", "The daily fine for this borrow record has already been assessed for today.");

        // --- Forbidden Errors (Business Rule Violations) ---
        public static Error ResponseInvalid(BorrowRecordStatus status) =>
            Error.Forbidden("BorrowRecord.ResponseInvalid", $"This request cannot be accepted because the current status is {status}.");
        public static Error ReturnInvalid(BorrowRecordStatus status) =>
            Error.Forbidden("BorrowRecord.ReturnInvalid", $"This copy cannot be returned because the current status is {status}.");
        public static Error RenewInvalid(BorrowRecordStatus status) =>
            Error.Forbidden("BorrowRecord.RenewInvalid", $"This record cannot be renewed because the current status is {status}.");
        public static Error CancellationInvalid(BorrowRecordStatus status) =>
            Error.Forbidden("BorrowRecord.CancellationInvalid", $"This record cannot be canceled because the current status is {status}.");
        public static Error RejectInvalid(BorrowRecordStatus status) =>
            Error.Forbidden("BorrowRecord.RejectInvalid", $"This request cannot be rejected because the current status is {status}.");
        public static Error PayFineInvalid => Error.Forbidden("BorrowRecord.PayFineInvalid", "Cannot process fine payment for this record.");
        public static Error CannotMarkAsLate => Error.Forbidden("BorrowRecord.CannotMarkAsLate", "This record cannot be marked as late in its current state.");
        public static Error CannotMarkAsLost => Error.Forbidden("BorrowRecord.CannotMarkAsLost", "This record cannot be marked as lost in its current state.");
        public static Error AlreadyPickedup => Error.Forbidden("BorrowRecord.AlreadyPickedUp", "This borrow record has already been picked up.");
    }
}
