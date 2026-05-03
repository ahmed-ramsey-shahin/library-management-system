using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Users.Queries.GetLibrariansByCategory
{
    public sealed record GetLibrariansByCategoryQuery(Guid CategoryId, int PageSize, int Page) : ICachedQuery<Result<PaginatedList<LibrarianSummaryDto>>>
    {
        public string CacheKey => $"users:librarians:category{CategoryId}:{PageSize}:{Page}";

        public string[] Tags => ["user", "librarian"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
