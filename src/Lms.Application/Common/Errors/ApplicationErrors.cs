using Lms.Domain.Common.Results;

namespace Lms.Application.Common.Errors
{
    public static class ApplicationErrors
    {
        public static Error BookNotFound => Error.NotFound("ApplicationErrors.Book.BookNotFound", "The required book was not found.");
        public static Error BookCopyNotFound => Error.NotFound("ApplicationErrors.BookCopy.BookCopyNotFound", "The required book copy was not found.");

        public static Error GenreNameLength => Error.Validation("ApplicationErrors.Genre.GenreNameLength", "Genre name length cannot exceed 50 characters.");
        public static Error GenreAlreadyExists => Error.Conflict("ApplicationErrors.Genre.GenreAlreadyExists", "This genre already exists.");
        public static Error GenreNotFound => Error.NotFound("ApplicationErrors.Genre.GenreNotFound", "The required genre was not found.");

        public static Error KeywordNameLength => Error.Validation("ApplicationErrors.Keyword.KeywordNameLength", "Keyword name length cannot exceed 50 characters.");
        public static Error KeywordAlreadyExists => Error.Conflict("ApplicationErrors.Keyword.KeywordAlreadyExists", "This keyword already exists.");
        public static Error KeywordNotFound => Error.NotFound("ApplicationErrors.Keyword.KeywordNotFound", "The required keyword was not found.");

        public static Error AudienceNameLength => Error.Validation("ApplicationErrors.Audience.AudienceNameLength", "Audience name length cannot exceed 50 characters.");
        public static Error AudienceAlreadyExists => Error.Conflict("ApplicationErrors.Audience.AudienceAlreadyExists", "This audience already exists.");
        public static Error AudienceNotFound => Error.NotFound("ApplicationErrors.Audience.AudienceNotFound", "The required audience was not found.");

        public static Error AuthorNameLength => Error.Validation("ApplicationErrors.Author.AuthorNameLength", "Author name length cannot exceed 50 characters.");
        public static Error AuthorAlreadyExists => Error.Conflict("ApplicationErrors.Author.AuthorAlreadyExists", "This author already exists.");
        public static Error AuthorNotFound => Error.NotFound("ApplicationErrors.Author.AuthorNotFound", "The required author was not found.");

        public static Error CategoryNameLength => Error.Validation("ApplicationErrors.Category.CategoryNameLength", "Category name length cannot exceed 50 characters.");
        public static Error CategoryAlreadyExists => Error.Conflict("ApplicationErrors.Category.CategoryAlreadyExists", "This category already exists.");
        public static Error CategoryNotFound => Error.NotFound("ApplicationErrors.Category.CategoryNotFound", "The required category was not found.");

        public static Error PublisherNameLength => Error.Validation("ApplicationErrors.Publisher.PublisherNameLength", "Publisher name length cannot exceed 50 characters.");
        public static Error PublisherAlreadyExists => Error.Conflict("ApplicationErrors.Publisher.PublisherAlreadyExists", "This publisher already exists.");
        public static Error PublisherNotFound => Error.NotFound("ApplicationErrors.Publisher.PublisherNotFound", "The required publisher was not found.");

        public static Error ThemeNameLength => Error.Validation("ApplicationErrors.Theme.ThemeNameLength", "Theme name length cannot exceed 50 characters.");
        public static Error ThemeAlreadyExists => Error.Conflict("ApplicationErrors.Theme.ThemeAlreadyExists", "This theme already exists.");
        public static Error ThemeNotFound => Error.NotFound("ApplicationErrors.Theme.ThemeNotFound", "The required theme was not found.");

        public static Error BookIsbnInvalid => Error.Validation("ApplicationErrors.Book.Isbn.Invalid", "The provided ISBN is invalid.");
        public static Error BookIssnInvalid => Error.Validation("ApplicationErrors.Book.Issn.Invalid", "The provided ISSN is invalid.");
        public static Error BookTitleLength => Error.Validation("ApplicationErrors.Book.Title.Length", "Book title length cannot exceed 255 characters.");
        public static Error BookDescriptionLength => Error.Validation("ApplicationErrors.Book.Description.Length", "Book description length cannot exceed 1024 characters.");
        public static Error IsbnAlreadyExists => Error.Conflict("ApplicationErrors.Book.Isbn.AlreadyExists", "A book with this ISBN already exists.");
        public static Error IssnAlreadyExists => Error.Conflict("ApplicationErrors.Book.Issn.AlreadyExists", "A book with this ISSN already exists.");

        public static Error BarcodeInvalid => Error.Validation("ApplicationErrors.BookCopy.Barcode.Invalid", "The barcode value must start with 'CPY-' followed by exactly 8 numeric digits.");
        public static Error LocationLength => Error.Validation("ApplicationErrors.BookCopy.Location.Length", "Theme location length cannot exceed 100 characters.");
        public static Error BarcodeAlreadyExists => Error.Conflict("ApplicationErrors.BookCopy.BarcodeAlreadyExists", "A book copy with this barcode already exists.");

        public static Error ConcurrencyConflict => Error.Conflict("ApplicationErrors.ConcurrencyConflict", "Could not perform the required operation because of a concurrency conflict.");
    }
}
