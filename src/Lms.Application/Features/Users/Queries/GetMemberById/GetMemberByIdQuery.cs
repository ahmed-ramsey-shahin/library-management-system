using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Users.Queries.GetMemberById
{
    public sealed record GetMemberByIdQuery(Guid MemberId) : ICachedQuery<Result<MemberDto>>
    {
        public string CacheKey => $"users:member:{MemberId}";

        public string[] Tags => ["user", "member"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
