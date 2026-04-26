using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Books.Queries.GetBooksByTitle
{
    public sealed record GetBooksByTitleQuery(string Title, int PageSize, int Page) : ICachedQuery<Result<PaginatedList<BookSummaryDto>>>
    {
        public string CacheKey => $"books:{Title.ToLower().Trim()}:{PageSize}:{Page}";

        public string[] Tags => ["book"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(30);
    }

}
