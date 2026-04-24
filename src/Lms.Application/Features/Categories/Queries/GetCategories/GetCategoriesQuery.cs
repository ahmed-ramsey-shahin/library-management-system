using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Categories.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Categories.Queries.GetCategories
{
    public sealed record GetCategoriesQuery : ICachedQuery<Result<List<CategoryDto>>>
    {
        public string CacheKey => "categories";

        public string[] Tags => ["category"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
