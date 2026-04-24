using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Publishers.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Publishers.Queries.GetPublisherByBookId
{
    public sealed record GetPublisherByBookIdQuery(Guid BookId) : ICachedQuery<Result<List<PublisherDto>>>
    {
        public string CacheKey => $"book:{BookId}:publisher";

        public string[] Tags => ["publisher", "book"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
