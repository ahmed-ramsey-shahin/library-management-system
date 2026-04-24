using Lms.Application.Features.Genres.Dtos;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Genres.Mappers
{
    public static class GenreMapper
    {
        public static GenreDto ToDto(this Genre genre)
        {
            return new()
            {
                GenreId = genre.Id,
                Name = genre.Name
            };
        }

        public static List<GenreDto> ToDto(this IEnumerable<Genre> genres)
        {
            return [.. genres.Select(ToDto)];
        }
    }
}
