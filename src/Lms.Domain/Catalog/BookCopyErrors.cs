using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public static class BookCopyErrors
    {
        // --- Validation Errors ---
        public static Error IdRequired => Error.Validation("BookCopy.IdRequired", "Copy ID is required.");
        public static Error BookIdRequired => Error.Validation("BookCopy.BookIdRequired", "Book ID is required.");
        public static Error BarcodeRequired => Error.Validation("BookCopy.BarcodeRequired", "Barcode is required.");
        public static Error LocationRequired => Error.Validation("BookCopy.LocationRequired", "Copy location is required.");
        public static Error AcquisitionDateInvalid => Error.Validation("BookCopy.AcquisitionDateInvalid", "Acquisition date cannot be in the future.");

        // --- Conflict Errors ---
        public static Error CannotDeleteBorrowedCopy => Error.Conflict("BookCopy.CannotDeleteBorrowedCopy", "The copy cannot be deleted because it is currently borrowed by a member.");
        public static Error CantChangeStateOfBorrowedBook => Error.Conflict("BookCopy.CantChangeStateOfBorrowedBook", "Status cannot be updated while the copy is borrowed. It must be returned first.");
        public static Error CopyNotGood => Error.Conflict("BookCopy.CopyNotGood", "This copy is not in a borrowable condition.");
    }
}
