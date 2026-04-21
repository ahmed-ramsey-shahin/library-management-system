using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public static class BookErrors
    {
        public static Error IdRequired => Error.Validation("Book.Id.Required", "Book ID is required.");
        public static Error IsbnRequired => Error.Validation("Book.Isbn.Required", "ISBN is required.");
        public static Error IssnRequired => Error.Validation("Book.Issn.Required", "ISSN is required.");
        public static Error TitleRequired => Error.Validation("Book.Title.Required", "Title is required.");
        public static Error LanguageRequired => Error.Validation("Book.Language.Required", "Language is required.");
        public static Error PublisherIdRequired => Error.Validation("Book.PublisherId.Required", "Publisher ID is required.");
        public static Error InvalidPublishingDate => Error.Validation("Book.PublishingDate.Invalid", "Publishing date must be in the past.");
        public static Error EditionRequired => Error.Validation("Book.Edition.Required", "Edition is required.");
        public static Error BorrowPricePerDayInvalid => Error.Validation("Book.BorrowPricePerDay.Invalid", "Borrow price per day must be greater than zero.");
        public static Error FinePerDayInvalid => Error.Validation("Book.FinePerDay.Invalid", "Fine per day must be greater than zero.");
        public static Error LostFeeInvalid => Error.Validation("Book.LostFee.Invalid", "Lost fee must be greater than zero.");
        public static Error DamageFeeInvalid => Error.Validation("Book.DamageFee.Invalid", "Damage fee must be greater than zero.");
        public static Error PageCountInvalid => Error.Validation("Book.PageCount.Invalid", "Page count must be greater than zero.");

        public static Error BookHasCopies => Error.Conflict("Book.HasCopies", "This operation cannot be completed because the book still has associated copies.");

        public static Error CategoryAlreadyAssigned => Error.Conflict("Book.CategoryAlreadyAssigned", "This operation cannot be completed because the category is already assigned.");
        public static Error CategoryNotAssigned => Error.Conflict("Book.CategoryNotAssigned", "This operation cannot be completed because the category is not assigned.");
        public static Error AuthorAlreadyAssigned => Error.Conflict("Book.AuthorAlreadyAssigned", "This operation cannot be completed because the author is already assigned.");
        public static Error AuthorNotAssigned => Error.Conflict("Book.AuthorNotAssigned", "This operation cannot be completed because the author is not assigned.");

        public static Error KeywordAlreadyAssigned => Error.Conflict("Book.KeywordAlreadyAssigned", "This operation cannot be completed because the keyword is already assigned.");
        public static Error KeywordNotAssigned => Error.Conflict("Book.KeywordNotAssigned", "This operation cannot be completed because the keyword is not assigned.");

        public static Error AudienceAlreadyAssigned => Error.Conflict("Book.AudienceAlreadyAssigned", "This operation cannot be completed because the audience is already assigned.");
        public static Error AudienceNotAssigned => Error.Conflict("Book.AudienceNotAssigned", "This operation cannot be completed because the audience is not assigned.");

        public static Error ThemeAlreadyAssigned => Error.Conflict("Book.ThemeAlreadyAssigned", "This operation cannot be completed because the theme is already assigned.");
        public static Error ThemeNotAssigned => Error.Conflict("Book.ThemeNotAssigned", "This operation cannot be completed because the theme is not assigned.");

        public static Error GenreAlreadyAssigned => Error.Conflict("Book.GenreAlreadyAssigned", "This operation cannot be completed because the genre is already assigned.");
        public static Error GenreNotAssigned => Error.Conflict("Book.GenreNotAssigned", "This operation cannot be completed because the genre is not assigned.");

        public static Error CopyNotFound => Error.NotFound("Book.CopyNotFound", "No copy of this book was found with the specified ID.");
    }
}
