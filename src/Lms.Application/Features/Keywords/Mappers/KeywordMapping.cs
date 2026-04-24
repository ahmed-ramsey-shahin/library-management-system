using Lms.Application.Features.Keywords.Dtos;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Keywords.Mappers
{
    public static class KeywordMapper
    {
        public static KeywordDto ToDto(this Keyword genre)
        {
            return new()
            {
                KeywordId = genre.Id,
                Name = genre.Name
            };
        }

        public static List<KeywordDto> ToDto(this IEnumerable<Keyword> genres)
        {
            return [.. genres.Select(ToDto)];
        }
    }
}
