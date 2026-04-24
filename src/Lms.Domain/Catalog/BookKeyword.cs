namespace Lms.Domain.Catalog
{
    public sealed class BookKeyword
    {
        public Guid BookId { get; }
        public Guid KeywordId { get; }

        private BookKeyword()
        {}

        internal BookKeyword(Guid bookId, Guid keywordId)
        {
            BookId = bookId;
            KeywordId = keywordId;
        }
    }
}
