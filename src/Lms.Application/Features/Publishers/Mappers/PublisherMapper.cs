using Lms.Application.Features.Publishers.Dtos;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Publishers.Mappers
{
    public static class PublisherMapper
    {
        public static PublisherDto ToDto(this Publisher publisher)
        {
            return new()
            {
                PublisherId = publisher.Id,
                Name = publisher.Name
            };
        }

        public static List<PublisherDto> ToDto(this IEnumerable<Publisher> publishers)
        {
            return [.. publishers.Select(ToDto)];
        }
    }
}
