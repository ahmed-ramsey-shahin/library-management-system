using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Users.Queries.GetAdmins
{
    public sealed record GetAdminsQuery(int PageSize, int Page) : ICachedQuery<Result<PaginatedList<AdminDto>>>
    {
        public string CacheKey => $"users:admins:{PageSize}:{Page}";

        public string[] Tags => ["user", "admins"];

        public TimeSpan Expiration => TimeSpan.FromHours(2);
    }
}
