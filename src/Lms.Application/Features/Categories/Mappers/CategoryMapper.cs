using Lms.Application.Features.Categories.Dtos;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Categories.Mappers
{
    public static class CategoryMapper
    {
        public static CategoryDto ToDto(this Category category)
        {
            return new()
            {
                CategoryId = category.Id,
                Name = category.Name
            };
        }

        public static List<CategoryDto> ToDto(this IEnumerable<Category> categorys)
        {
            return [.. categorys.Select(ToDto)];
        }
    }
}
