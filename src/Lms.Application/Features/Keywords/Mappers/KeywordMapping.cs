using Lms.Application.Features.Keywords.Dtos;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Keywords.Mappers
{
    public static class KeywordMapper
    {
        public static KeywordDto ToDto(this Keyword keyword)
        {
            return new()
            {
                KeywordId = keyword.Id,
                Name = keyword.Name
            };
        }

        public static List<KeywordDto> ToDto(this IEnumerable<Keyword> keywords)
        {
            return [.. keywords.Select(ToDto)];
        }
    }
}
