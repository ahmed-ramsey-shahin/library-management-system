using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public static class GenreErrors
    {
        public static Error IdRequired => Error.Validation("Genre.Id.Required", "Genre id is required.");
        public static Error NameRequired => Error.Validation("Genre.Name.Required", "Genre name is required.");
        public static Error GenreHasBooks => Error.Conflict("Genre.HasBooks", "This operation cannot be completed because this genre still has books associated with it.");
    }
}
