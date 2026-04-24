using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public sealed class Category : AuditableEntity
    {
        public Guid Id { get; }
        public string Name { get; private set; } = string.Empty;

        private Category()
        {}

        private Category(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Result<Category> Create(Guid id, string name)
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(CategoryErrors.IdRequired);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(CategoryErrors.NameRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new Category(id, name);
        }

        public Result<Updated> Update(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return CategoryErrors.NameRequired;
            }

            Name = name;
            return Result.Updated;
        }

        public Result<Deleted> Delete(int associatedBooks, int associatedLibrarians)
        {
            if (associatedBooks > 0)
            {
                return CategoryErrors.CategoryHasBooks;
            }

            if (associatedLibrarians > 0)
            {
                return CategoryErrors.CategoryHasLibrarians;
            }

            IsDeleted = true;
            return Result.Deleted;
        }
    }
}
