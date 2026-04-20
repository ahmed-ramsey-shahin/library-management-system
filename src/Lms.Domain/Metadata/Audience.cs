using Lms.Domain.Catalog;
using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public sealed class Audience : AuditableEntity
    {
        public Guid Id { get; }
        public string Name { get; private set; } = string.Empty;
        private readonly List<Book> _books = [];
        public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

        private Audience()
        {}

        private Audience(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Result<Audience> Create(Guid id, string name)
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(AudienceErrors.IdRequired);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(AudienceErrors.NameRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new Audience(id, name);
        }

        public Result<Updated> Update(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return AudienceErrors.NameRequired;
            }

            Name = name;
            return Result.Updated;
        }

        public Result<Deleted> Delete(int associatedBooks)
        {
            if (associatedBooks > 0)
            {
                return AudienceErrors.AudienceHasBooks;
            }

            IsDeleted = true;
            return Result.Deleted;
        }
    }
}
