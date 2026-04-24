namespace Lms.Domain.Catalog
{
    public sealed class BookGenre
    {
        public Guid BookId { get; }
        public Guid GenreId { get; }

        private BookGenre()
        {}

        internal BookGenre(Guid bookId, Guid genreId)
        {
            BookId = bookId;
            GenreId = genreId;
        }
    }
}
