using Lms.Application.Features.Audiences.Dtos;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Audiences.Mappers
{
    public static class AudienceMapper
    {
        public static AudienceDto ToDto(this Audience genre)
        {
            return new()
            {
                AudienceId = genre.Id,
                Name = genre.Name
            };
        }

        public static List<AudienceDto> ToDto(this IEnumerable<Audience> genres)
        {
            return [.. genres.Select(ToDto)];
        }
    }
}
