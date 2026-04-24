using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Audiences.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Audiences.Queries.GetAudiences
{
    public sealed record GetAudiencesQuery : ICachedQuery<Result<List<AudienceDto>>>
    {
        public string CacheKey => "audiences";

        public string[] Tags => ["audience"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
