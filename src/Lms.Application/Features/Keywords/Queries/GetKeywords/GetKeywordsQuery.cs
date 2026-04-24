using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Keywords.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Keywords.Queries.GetKeywords
{
    public sealed record GetKeywordsQuery : ICachedQuery<Result<List<KeywordDto>>>
    {
        public string CacheKey => "keywords";

        public string[] Tags => ["keyword"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
