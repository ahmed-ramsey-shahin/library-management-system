using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Audiences.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Audiences.Queries.GetAudiencesByBookId
{
    public sealed record GetAudiencesByBookIdQuery(Guid BookId) : ICachedQuery<Result<List<AudienceDto>>>
    {
        public string CacheKey => $"book:{BookId}:audiences";

        public string[] Tags => ["audience", "book"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
