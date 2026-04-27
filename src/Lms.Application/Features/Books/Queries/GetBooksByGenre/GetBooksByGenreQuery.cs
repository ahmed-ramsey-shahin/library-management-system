using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Books.Queries.GetBooksByGenre
{
    public sealed record GetBooksByGenreQuery(Guid GenreId, int PageSize, int Page) : ICachedQuery<Result<PaginatedList<BookSummaryDto>>>
    {
        public string CacheKey => $"genres:{GenreId}:book:{PageSize}:{Page}";

        public string[] Tags => ["book"];

        public TimeSpan Expiration => TimeSpan.FromHours(1);
    }
}
