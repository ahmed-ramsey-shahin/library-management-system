namespace Lms.Domain.Identity
{
    public sealed class LibrarianCategory
    {
        public Guid UserId { get; }
        public Guid CategoryId { get; }

        private LibrarianCategory()
        {}

        internal LibrarianCategory(Guid userId, Guid categoryId)
        {
            UserId = userId;
            CategoryId = categoryId;
        }
    }
}
