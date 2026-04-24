using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public sealed class Genre : AuditableEntity
    {
        public Guid Id { get; }
        public string Name { get; private set; } = string.Empty;

        private Genre()
        {}

        private Genre(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Result<Genre> Create(Guid id, string name)
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(GenreErrors.IdRequired);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(GenreErrors.NameRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new Genre(id, name);
        }

        public Result<Updated> Update(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GenreErrors.NameRequired;
            }

            Name = name;
            return Result.Updated;
        }

        public Result<Deleted> Delete(int associatedBooks)
        {
            if (associatedBooks > 0)
            {
                return GenreErrors.GenreHasBooks;
            }

            IsDeleted = true;
            return Result.Deleted;
        }
    }
}
