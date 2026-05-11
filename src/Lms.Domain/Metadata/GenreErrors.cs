using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public static class GenreErrors
    {
        public static Error IdRequired => Error.Validation("Genre.IdRequired", "Genre ID is required.");
        public static Error NameRequired => Error.Validation("Genre.NameRequired", "Genre name is required.");
        public static Error GenreHasBooks => Error.Conflict("Genre.HasBooks", "Cannot delete or modify the genre because it is still associated with books.");
    }
}
