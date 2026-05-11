using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation
{
    public static class FineErrors
    {
        // --- Validation Errors ---
        public static Error IdRequired => Error.Validation("Fine.IdRequired", "Fine ID is required.");
        public static Error MemberIdRequired => Error.Validation("Fine.MemberIdRequired", "Member ID is required.");
        public static Error BorrowRecordIdRequired => Error.Validation("Fine.BorrowRecordIdRequired", "Borrow record ID is required.");
        public static Error AmountInvalid => Error.Validation("Fine.AmountInvalid", "The fine amount must be greater than zero.");
        public static Error DescriptionRequired => Error.Validation("Fine.DescriptionRequired", "Fine description is required.");
        public static Error FineDateInvalid => Error.Validation("Fine.FineDateInvalid", "The fine date cannot be in the future.");

        // --- Conflict Errors ---
        public static Error CannotChangeAmount => Error.Conflict("Fine.CannotChangeAmount", "The amount of this fine cannot be modified.");
        public static Error CannotWaivePaidFines => Error.Conflict("Fine.CannotWaivePaidFines", "A paid fine cannot be waived.");
    }
}
