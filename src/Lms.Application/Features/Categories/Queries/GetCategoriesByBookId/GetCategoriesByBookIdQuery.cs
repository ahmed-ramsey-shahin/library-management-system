using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Categories.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Categories.Queries.GetCategoriesByBookId
{
    public sealed record GetCategoriesByBookIdQuery(Guid BookId) : ICachedQuery<Result<List<CategoryDto>>>
    {
        public string CacheKey => $"book:{BookId}:categories";

        public string[] Tags => ["category", "book"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
