using Lms.Domain.Catalog;

namespace Lms.Domain.Identity
{
    public sealed class LibrarianCategory
    {
        public Guid UserId { get; }
        public Guid CategoryId { get; }

        public User Librarian { get; } = null!;
        public Category Category { get; } = null!;

        private LibrarianCategory()
        {}

        internal LibrarianCategory(Guid userId, Guid categoryId)
        {
            UserId = userId;
            CategoryId = categoryId;
        }
    }
}
