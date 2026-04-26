using Lms.Domain.Metadata;

namespace Lms.Domain.Catalog
{
    public sealed class BookGenre
    {
        public Guid BookId { get; }
        public Guid GenreId { get; }

        public Book Book { get; } = null!;
        public Genre Genre { get; } = null!;

        private BookGenre()
        {}

        internal BookGenre(Guid bookId, Guid genreId)
        {
            BookId = bookId;
            GenreId = genreId;
        }
    }
}
