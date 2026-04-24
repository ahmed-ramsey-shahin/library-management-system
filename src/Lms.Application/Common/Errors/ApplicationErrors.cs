using Lms.Domain.Common.Results;

namespace Lms.Application.Common.Errors
{
    public static class ApplicationErrors
    {
        public static Error BookNotFound => Error.Validation("ApplicationErrors.Book.BookNotFound", "The required book was not found.");

        public static Error GenreNameLength => Error.Validation("ApplicationErrors.Genre.GenreNameLength", "Genre name length cannot exceed 50 characetrs.");
        public static Error GenreAlreadyExists => Error.Validation("ApplicationErrors.Genre.GenreAlreadyExists", "This genre already exists.");
        public static Error GenreNotFound => Error.Validation("ApplicationErrors.Genre.GenreNotFound", "The required genre was not found.");

        public static Error KeywordNameLength => Error.Validation("ApplicationErrors.Keyword.KeywordNameLength", "Keyword name length cannot exceed 50 characetrs.");
        public static Error KeywordAlreadyExists => Error.Validation("ApplicationErrors.Keyword.KeywordAlreadyExists", "This keyword already exists.");
        public static Error KeywordNotFound => Error.Validation("ApplicationErrors.Keyword.KeywordNotFound", "The required keyword was not found.");

        public static Error AudienceNameLength => Error.Validation("ApplicationErrors.Audience.AudienceNameLength", "Audience name length cannot exceed 50 characetrs.");
        public static Error AudienceAlreadyExists => Error.Validation("ApplicationErrors.Audience.AudienceAlreadyExists", "This audience already exists.");
        public static Error AudienceNotFound => Error.Validation("ApplicationErrors.Audience.AudienceNotFound", "The required audience was not found.");

        public static Error AuthorNameLength => Error.Validation("ApplicationErrors.Author.AuthorNameLength", "Author name length cannot exceed 50 characetrs.");
        public static Error AuthorAlreadyExists => Error.Validation("ApplicationErrors.Author.AuthorAlreadyExists", "This author already exists.");
        public static Error AuthorNotFound => Error.Validation("ApplicationErrors.Author.AuthorNotFound", "The required author was not found.");

        public static Error CategoryNameLength => Error.Validation("ApplicationErrors.Category.CategoryNameLength", "Category name length cannot exceed 50 characetrs.");
        public static Error CategoryAlreadyExists => Error.Validation("ApplicationErrors.Category.CategoryAlreadyExists", "This category already exists.");
        public static Error CategoryNotFound => Error.Validation("ApplicationErrors.Category.CategoryNotFound", "The required category was not found.");

        public static Error PublisherNameLength => Error.Validation("ApplicationErrors.Publisher.PublisherNameLength", "Publisher name length cannot exceed 50 characetrs.");
        public static Error PublisherAlreadyExists => Error.Validation("ApplicationErrors.Publisher.PublisherAlreadyExists", "This publisher already exists.");
        public static Error PublisherNotFound => Error.Validation("ApplicationErrors.Publisher.PublisherNotFound", "The required publisher was not found.");

        public static Error ThemeNameLength => Error.Validation("ApplicationErrors.Theme.ThemeNameLength", "Theme name length cannot exceed 50 characetrs.");
        public static Error ThemeAlreadyExists => Error.Validation("ApplicationErrors.Theme.ThemeAlreadyExists", "This theme already exists.");
        public static Error ThemeNotFound => Error.Validation("ApplicationErrors.Theme.ThemeNotFound", "The required theme was not found.");
    }
}
