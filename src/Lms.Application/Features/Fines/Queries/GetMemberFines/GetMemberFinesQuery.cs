using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Fines.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Fines.Queries.GetMemberFines
{
    public record GetMemberFinesQuery(Guid MemberId, int Page, int PageSize) : ICachedQuery<Result<PaginatedList<FineDto>>>
    {
        public string CacheKey => $"fines:member:{MemberId}:{PageSize}:{Page}";

        public string[] Tags => ["fines"];

        public TimeSpan Expiration => TimeSpan.FromHours(2);
    }
}
