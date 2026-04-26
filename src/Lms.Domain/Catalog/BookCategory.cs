namespace Lms.Domain.Catalog
{
    public sealed class BookCategory
    {
        public Guid BookId { get; }
        public Guid CategoryId { get; }

        public Book Book { get; } = null!;
        public Category Category { get; } = null!;

        private BookCategory()
        {}

        internal BookCategory(Guid bookId, Guid categoryId)
        {
            BookId = bookId;
            CategoryId = categoryId;
        }
    }
}
