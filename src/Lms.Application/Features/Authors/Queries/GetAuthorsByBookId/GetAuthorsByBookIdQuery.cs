using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Authors.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Authors.Queries.GetAuthorsByBookId
{
    public sealed record GetAuthorsByBookIdQuery(Guid BookId) : ICachedQuery<Result<List<AuthorDto>>>
    {
        public string CacheKey => $"book:{BookId}:authors";

        public string[] Tags => ["author", "book"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
