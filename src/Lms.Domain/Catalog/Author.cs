using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public sealed class Author : AuditableEntity
    {
        public Guid Id { get; }
        public string Name { get; private set; } = string.Empty;

        private Author()
        {}

        private Author(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Result<Author> Create(Guid id, string name)
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(AuthorErrors.IdRequired);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(AuthorErrors.NameRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new Author(id, name);
        }

        public Result<Updated> Update(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return AuthorErrors.NameRequired;
            }

            Name = name;
            return Result.Updated;
        }

        public Result<Deleted> Delete(int associatedBooks)
        {
            if (associatedBooks > 0)
            {
                return AuthorErrors.AuthorHasBooks;
            }

            IsDeleted = true;
            return Result.Deleted;
        }
    }
}
