using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public static class CategoryErrors
    {
        public static Error IdRequired => Error.Validation("Category.Id.Required", "Category id is required.");
        public static Error NameRequired => Error.Validation("Category.Name.Required", "Category name is required.");
        public static Error CategoryHasBooks => Error.Conflict("Category.HasBooks", "This operation cannot be completed because the category still contains books.");
        public static Error CategoryHasLibrarians => Error.Conflict("Category.HasLibrarians", "This operation cannot be completed because the category still has associated librarians.");
    }
}
