using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Users.Queries.GetUserByEmail
{
    public sealed record GetUserByEmailQuery(string Email) : ICachedQuery<Result<UserDto>>
    {
        public string CacheKey => $"users:{Email}";

        public string[] Tags => ["user"];

        public TimeSpan Expiration => TimeSpan.FromHours(2);
    }
}
