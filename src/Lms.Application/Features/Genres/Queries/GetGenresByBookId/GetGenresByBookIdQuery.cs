using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Genres.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Genres.Queries.GetGenresByBookId
{
    public sealed record GetGenresByBookIdQuery(Guid BookId) : ICachedQuery<Result<List<GenreDto>>>
    {
        public string CacheKey => $"book:{BookId}:genres";

        public string[] Tags => ["genre", "book"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
