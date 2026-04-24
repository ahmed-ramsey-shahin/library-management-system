using Lms.Application.Features.Themes.Dtos;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Themes.Mappers
{
    public static class ThemeMapper
    {
        public static ThemeDto ToDto(this Theme theme)
        {
            return new()
            {
                ThemeId = theme.Id,
                Name = theme.Name
            };
        }

        public static List<ThemeDto> ToDto(this IEnumerable<Theme> themes)
        {
            return [.. themes.Select(ToDto)];
        }
    }
}
