using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Books.Queries.GetBookCopies
{
    public sealed record GetBookCopiesQuery(Guid BookId) : ICachedQuery<Result<List<BookCopySummaryDto>>>
    {
        public string CacheKey => $"book:{BookId}:copies";

        public string[] Tags => ["book-copy", "book"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(5);
    }
}
