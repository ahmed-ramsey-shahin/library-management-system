using Lms.Domain.Metadata;

namespace Lms.Domain.Catalog
{
    public sealed class BookKeyword
    {
        public Guid BookId { get; }
        public Guid KeywordId { get; }

        public Book Book { get; } = null!;
        public Keyword Keyword { get; } = null!;

        private BookKeyword()
        {}

        internal BookKeyword(Guid bookId, Guid keywordId)
        {
            BookId = bookId;
            KeywordId = keywordId;
        }
    }
}
