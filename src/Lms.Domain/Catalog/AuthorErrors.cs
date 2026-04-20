using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public static class AuthorErrors
    {
        public static Error IdRequired => Error.Validation("Author.Id.Required", "Author id is required.");
        public static Error NameRequired => Error.Validation("Author.Name.Required", "Author name is required.");
        public static Error AuthorHasBooks => Error.Conflict("Author.HasBooks", "This operation cannot be completed because the author still has books associated with it.");
    }
}
