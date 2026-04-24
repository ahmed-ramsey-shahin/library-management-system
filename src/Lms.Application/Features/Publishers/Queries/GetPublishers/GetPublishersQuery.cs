using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Publishers.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Publishers.Queries.GetPublishers
{
    public sealed record GetPublishersQuery : ICachedQuery<Result<List<PublisherDto>>>
    {
        public string CacheKey => "publishers";

        public string[] Tags => ["publisher"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
