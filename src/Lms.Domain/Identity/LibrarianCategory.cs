using Lms.Domain.Catalog;

namespace Lms.Domain.Identity
{
    public sealed class LibrarianCategory
    {
        public Guid UserId { get; }
        public Guid CategoryId { get; }
        public User User { get; internal set; } = null!;
        public Category Category { get; internal set; } = null!;

        private LibrarianCategory()
        {}

        internal LibrarianCategory(Guid userId, Guid categoryId)
        {
            UserId = userId;
            CategoryId = categoryId;
        }
    }
}
