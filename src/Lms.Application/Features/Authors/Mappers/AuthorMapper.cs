using Lms.Application.Features.Authors.Dtos;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Authors.Mappers
{
    public static class AuthorMapper
    {
        public static AuthorDto ToDto(this Author author)
        {
            return new()
            {
                AuthorId = author.Id,
                Name = author.Name
            };
        }

        public static List<AuthorDto> ToDto(this IEnumerable<Author> authors)
        {
            return [.. authors.Select(ToDto)];
        }
    }
}
