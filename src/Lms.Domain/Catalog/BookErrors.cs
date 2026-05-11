using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public static class BookErrors
    {
        // --- Validation Errors (Input/Data Constraints) ---
        public static Error IdRequired => Error.Validation("Book.IdRequired", "The book ID is required.");
        public static Error IsbnRequired => Error.Validation("Book.IsbnRequired", "The ISBN is required.");
        public static Error IssnRequired => Error.Validation("Book.IssnRequired", "The ISSN is required.");
        public static Error TitleRequired => Error.Validation("Book.TitleRequired", "The book title is required.");
        public static Error LanguageRequired => Error.Validation("Book.LanguageRequired", "The language is required.");
        public static Error PublisherIdRequired => Error.Validation("Book.PublisherIdRequired", "The publisher ID is required.");
        public static Error InvalidPublishingDate => Error.Validation("Book.InvalidPublishingDate", "The publishing date must be in the past.");
        public static Error EditionRequired => Error.Validation("Book.EditionRequired", "The edition is required.");

        public static Error BorrowPricePerDayInvalid => Error.Validation("Book.BorrowPricePerDayInvalid", "Borrow price per day must be a positive value.");
        public static Error FinePerDayInvalid => Error.Validation("Book.FinePerDayInvalid", "Fine per day must be a positive value.");
        public static Error LostFeeInvalid => Error.Validation("Book.LostFeeInvalid", "Lost fee must be a positive value.");
        public static Error DamageFeeInvalid => Error.Validation("Book.DamageFeeInvalid", "Damage fee must be a positive value.");
        public static Error PageCountInvalid => Error.Validation("Book.PageCountInvalid", "Page count must be greater than zero.");

        public static Error CategoryIdRequired => Error.Validation("Book.CategoryIdRequired", "The category ID is required.");
        public static Error ThemeIdRequired => Error.Validation("Book.ThemeIdRequired", "The theme ID is required.");
        public static Error KeywordIdRequired => Error.Validation("Book.KeywordIdRequired", "The keyword ID is required.");
        public static Error GenreIdRequired => Error.Validation("Book.GenreIdRequired", "The genre ID is required.");
        public static Error AudienceIdRequired => Error.Validation("Book.AudienceIdRequired", "The audience ID is required.");
        public static Error AuthorIdRequired => Error.Validation("Book.AuthorIdRequired", "The author ID is required.");

        // --- Conflict Errors (Business Logic/State Constraints) ---
        public static Error BookHasCopies => Error.Conflict("Book.HasCopies", "Cannot delete or modify the book because it still has associated copies.");

        // --- NotFound Errors (Missing Resources) ---
        public static Error CopyNotFound => Error.NotFound("Book.CopyNotFound", "The specified book copy was not found.");
        public static Error NoAvailableCopies => Error.NotFound("Book.NoAvailableCopies", "There are currently no copies of this book available.");
    }
}
