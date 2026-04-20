using Lms.Domain.Catalog;
using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Metadata
{
    public sealed class Keyword : AuditableEntity
    {
        public Guid Id { get; }
        public string Name { get; private set; } = string.Empty;
        private readonly List<Book> _books = [];
        public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

        private Keyword()
        {}

        private Keyword(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Result<Keyword> Create(Guid id, string name)
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(KeywordErrors.IdRequired);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(KeywordErrors.NameRequired);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new Keyword(id, name);
        }

        public Result<Updated> Update(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return KeywordErrors.NameRequired;
            }

            Name = name;
            return Result.Updated;
        }

        public Result<Deleted> Delete()
        {
            if (_books.Count > 0)
            {
                return KeywordErrors.KeywordHasBooks;
            }

            IsDeleted = true;
            return Result.Deleted;
        }
    }
}
