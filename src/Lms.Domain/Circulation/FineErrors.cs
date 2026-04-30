using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation
{
    public static class FineErrors
    {
        public static Error IdRequired => Error.Validation("Fine.Id.Required", "Fine ID is required.");
        public static Error MemberIdRequired => Error.Validation("Fine.MemberId.Required", "Member ID is required.");
        public static Error BorrowRecordId => Error.Validation("Fine.BorrowRecordId.Required", "Borrow record ID is required.");
        public static Error AmountInvalid => Error.Validation("Fine.Amount.Invalid", "Fine fee must be greater than zero.");
        public static Error DescriptionRequired => Error.Validation("Fine.Description.Required", "Fine description is required.");
        public static Error FineDateInvalid => Error.Validation("Fine.FineDate.Invalid", "The fine date cannot be in the future.");
        public static Error FineUnpaid => Error.Forbidden("Fine.Unpaid", "Cannot deleted an unpaid fine.");
        public static Error FineAlreadyPaid => Error.Conflict("Fine.AlreadyPaid", "This fine is already paid.");
        public static Error CannotChangeAmount => Error.Conflict("Fine.CannotChangeAmount", "The amount of this fine can not be changed.");
        public static Error CannotWaivePaidFines => Error.Conflict("Fine.CannotWaivePaidFines", "This fine is paid and can not be waived.");
    }
}
