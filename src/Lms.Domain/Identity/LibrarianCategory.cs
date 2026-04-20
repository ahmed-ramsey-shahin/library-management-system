using Lms.Domain.Catalog;

namespace Lms.Domain.Identity
{
    public sealed class LibrarianCategory
    {
        public Guid UserId { get; }
        public Guid CategoryId { get; }
        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;

        private LibrarianCategory()
        {}

        internal LibrarianCategory(Guid userId, Guid categoryId)
        {
            UserId = userId;
            CategoryId = categoryId;
        }
    }
}
