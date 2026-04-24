using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Keywords.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Keywords.Queries.GetKeywordsByBookId
{
    public sealed record GetKeywordsByBookIdQuery(Guid BookId) : ICachedQuery<Result<List<KeywordDto>>>
    {
        public string CacheKey => $"book:{BookId}:keywords";

        public string[] Tags => ["keyword", "book"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
