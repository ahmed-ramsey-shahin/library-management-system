using Lms.Application.Features.Audiences.Dtos;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Audiences.Mappers
{
    public static class AudienceMapper
    {
        public static AudienceDto ToDto(this Audience audience)
        {
            return new()
            {
                AudienceId = audience.Id,
                Name = audience.Name
            };
        }

        public static List<AudienceDto> ToDto(this IEnumerable<Audience> audiences)
        {
            return [.. audiences.Select(ToDto)];
        }
    }
}
