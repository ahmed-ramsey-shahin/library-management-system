using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Users.Queries.GetMembers
{
    public sealed record GetMembersQuery(int PageSize, int Page) : ICachedQuery<Result<PaginatedList<MemberSummaryDto>>>
    {
        public string CacheKey => $"users:members:{PageSize}:{Page}";

        public string[] Tags => ["user", "members"];

        public TimeSpan Expiration => TimeSpan.FromHours(2);
    }
}
