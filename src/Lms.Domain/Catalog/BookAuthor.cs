namespace Lms.Domain.Catalog
{
    public sealed class BookAuthor
    {
        public Guid BookId { get; }
        public Guid AuthorId { get; }

        private BookAuthor()
        {}

        internal BookAuthor(Guid bookId, Guid authorId)
        {
            BookId = bookId;
            AuthorId = authorId;
        }
    }
}
