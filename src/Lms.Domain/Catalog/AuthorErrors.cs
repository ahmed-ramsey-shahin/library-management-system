using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public static class AuthorErrors
    {
        public static Error IdRequired => Error.Validation("Author.IdRequired", "Author ID is required.");
        public static Error NameRequired => Error.Validation("Author.NameRequired", "Author name is required.");
        public static Error AuthorHasBooks => Error.Conflict("Author.HasBooks", "The author cannot be deleted or modified because they are still associated with books in the catalog.");
    }
}
