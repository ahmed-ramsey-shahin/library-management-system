namespace Lms.Domain.Catalog
{
    public sealed class BookAudience
    {
        public Guid BookId { get; }
        public Guid AudienceId { get; }

        private BookAudience()
        {}

        internal BookAudience(Guid bookId, Guid audienceId)
        {
            BookId = bookId;
            AudienceId = audienceId;
        }
    }
}
