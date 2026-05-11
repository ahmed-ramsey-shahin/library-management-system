using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public static class CategoryErrors
    {
        public static Error IdRequired => Error.Validation("Category.IdRequired", "Category ID is required.");
        public static Error NameRequired => Error.Validation("Category.NameRequired", "Category name is required.");
        public static Error CategoryHasBooks => Error.Conflict("Category.HasBooks", "The category cannot be deleted because it still contains books.");
        public static Error CategoryHasLibrarians => Error.Conflict("Category.HasLibrarians", "The category cannot be deleted because it is still associated with librarians.");
    }
}
