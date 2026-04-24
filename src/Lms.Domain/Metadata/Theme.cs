using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public sealed class Theme : AuditableEntity
    {
        public Guid Id { get; }
        public string Name { get; private set; } = string.Empty;

        private Theme()
        {}

        private Theme(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Result<Theme> Create(Guid id, string name)
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(ThemeErrors.IdRequired);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(ThemeErrors.NameRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new Theme(id, name);
        }

        public Result<Updated> Update(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ThemeErrors.NameRequired;
            }

            Name = name;
            return Result.Updated;
        }

        public Result<Deleted> Delete(int associatedBooks)
        {
            if (associatedBooks > 0)
            {
                return ThemeErrors.ThemeHasBooks;
            }

            IsDeleted = true;
            return Result.Deleted;
        }
    }
}
