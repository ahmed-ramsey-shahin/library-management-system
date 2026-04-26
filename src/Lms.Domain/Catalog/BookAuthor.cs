namespace Lms.Domain.Catalog
{
    public sealed class BookAuthor
    {
        public Guid BookId { get; }
        public Guid AuthorId { get; }

        public Book Book { get; } = null!;
        public Author Author { get; } = null!;

        private BookAuthor()
        {}

        internal BookAuthor(Guid bookId, Guid authorId)
        {
            BookId = bookId;
            AuthorId = authorId;
        }
    }
}
