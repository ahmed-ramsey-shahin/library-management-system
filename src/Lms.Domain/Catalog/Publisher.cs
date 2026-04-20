using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public sealed class Publisher : AuditableEntity
    {
        public Guid Id { get; }
        public string Name { get; private set; } = string.Empty;
        private readonly List<Book> _books = [];
        public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

        private Publisher()
        {}

        private Publisher(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Result<Publisher> Create(Guid id, string name)
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(PublisherErrors.IdRequired);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(PublisherErrors.NameRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new Publisher(id, name);
        }

        public Result<Updated> Update(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return PublisherErrors.NameRequired;
            }

            Name = name;
            return Result.Updated;
        }

        public Result<Deleted> Delete()
        {
            if (_books.Count > 0)
            {
                return PublisherErrors.PublisherHasBooks;
            }

            IsDeleted = true;
            return Result.Deleted;
        }
    }
}
