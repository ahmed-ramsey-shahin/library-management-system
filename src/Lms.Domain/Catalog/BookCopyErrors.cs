using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public static class BookCopyErrors
    {
        public static Error IdRequired => Error.Validation("BookCopy.Id.Required", "Copy ID is required.");
        public static Error BookIdRequired => Error.Validation("BookCopy.BookId.Required", "Book ID is required.");
        public static Error BarcodeRequired => Error.Validation("BookCopy.Barcode.Required", "Barcode is required.");
        public static Error LocationRequired => Error.Validation("BookCopy.Location.Required", "Copy location is required.");
        public static Error AcquisitionDateInvalid => Error.Validation("BookCopy.AcquisitionDate.Invalid", "Acquisition date cannot be in the future.");
        public static Error CannotDeleteBorrowedCopy => Error.NotFound("BookCopy.CannotDeleteBorrowedCopy", "This operation cannot be completed because this copy is borrowed by a member.");
        public static Error CantChangeStateOfBorrowedBook => Error.Conflict("BookCopy.CantChangeStateOfBorrowedBook", "This copy is currently borrowed. Return it before changing its status.");
        public static Error CopyNotGood => Error.Conflict("BookCopy.CopyNotGood", "This copy is not in a good condition to be borrowed.");
    }
}
