using Lms.Application.Features.Authors.Dtos;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Authors.Mappers
{
    public static class AuthorMapper
    {
        public static AuthorDto ToDto(this Author genre)
        {
            return new()
            {
                AuthorId = genre.Id,
                Name = genre.Name
            };
        }

        public static List<AuthorDto> ToDto(this IEnumerable<Author> genres)
        {
            return [.. genres.Select(ToDto)];
        }
    }
}
