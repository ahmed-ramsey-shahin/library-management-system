using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Themes.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Themes.Queries.GetThemesByBookId
{
    public sealed record GetThemesByBookIdQuery(Guid BookId) : ICachedQuery<Result<List<ThemeDto>>>
    {
        public string CacheKey => $"book:{BookId}:themes";

        public string[] Tags => ["theme", "book"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
