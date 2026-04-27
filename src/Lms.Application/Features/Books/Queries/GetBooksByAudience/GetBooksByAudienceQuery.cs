using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Books.Queries.GetBooksByAudience
{
    public sealed record GetBooksByAudienceQuery(Guid AudienceId, int PageSize, int Page) : ICachedQuery<Result<PaginatedList<BookSummaryDto>>>
    {
        public string CacheKey => $"audiences:{AudienceId}:book:{PageSize}:{Page}";

        public string[] Tags => ["book"];

        public TimeSpan Expiration => TimeSpan.FromHours(1);
    }
}
