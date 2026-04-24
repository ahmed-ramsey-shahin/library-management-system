using Lms.Domain.Common.Results;

namespace Lms.Application.Common.Errors
{
    public static class ApplicationErrors
    {
        public static Error GenreNameLength => Error.Validation("ApplicationErrors.Genre.GenreNameLength", "Genre name length cannot exceed 50 characetrs.");
        public static Error GenreAlreadyExists => Error.Validation("ApplicationErrors.Genre.GenreAlreadyExists", "This genre already exists.");
        public static Error GenreNotFound => Error.Validation("ApplicationErrors.Genre.GenreNotFound", "The required genre was not found.");
        public static Error BookNotFound => Error.Validation("ApplicationErrors.Book.BookNotFound", "The required book was not found.");
    }
}
