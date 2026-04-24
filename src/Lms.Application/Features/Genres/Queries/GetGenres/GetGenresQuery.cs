using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Genres.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Genres.Queries.GetGenres
{
    public sealed record GetGenresQuery : ICachedQuery<Result<List<GenreDto>>>
    {
        public string CacheKey => "genres";

        public string[] Tags => ["genre"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
