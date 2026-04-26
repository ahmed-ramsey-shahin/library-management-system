using Lms.Domain.Metadata;

namespace Lms.Domain.Catalog
{
    public sealed class BookAudience
    {
        public Guid BookId { get; }
        public Guid AudienceId { get; }

        public Book Book { get; } = null!;
        public Audience Audience { get; } = null!;

        private BookAudience()
        {}

        internal BookAudience(Guid bookId, Guid audienceId)
        {
            BookId = bookId;
            AudienceId = audienceId;
        }
    }
}
