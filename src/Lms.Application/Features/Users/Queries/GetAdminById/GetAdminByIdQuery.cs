using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Users.Queries.GetAdminById
{
    public sealed record GetAdminByIdQuery(Guid AdminId) : ICachedQuery<Result<AdminDto>>
    {
        public string CacheKey => $"users:admin:{AdminId}";

        public string[] Tags => ["user", "admin"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
