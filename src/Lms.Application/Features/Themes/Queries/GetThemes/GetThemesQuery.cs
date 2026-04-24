using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Themes.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Themes.Queries.GetThemes
{
    public sealed record GetThemesQuery : ICachedQuery<Result<List<ThemeDto>>>
    {
        public string CacheKey => "themes";

        public string[] Tags => ["theme"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
