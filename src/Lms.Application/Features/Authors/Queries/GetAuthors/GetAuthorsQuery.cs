using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Authors.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Authors.Queries.GetAuthors
{
    public sealed record GetAuthorsQuery : ICachedQuery<Result<List<AuthorDto>>>
    {
        public string CacheKey => "authors";

        public string[] Tags => ["author"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
