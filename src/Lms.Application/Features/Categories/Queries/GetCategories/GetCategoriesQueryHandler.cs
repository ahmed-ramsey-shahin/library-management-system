using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Categories.Dtos;
using Lms.Application.Features.Categories.Mappers;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Categories.Queries.GetCategories
{
    public sealed class GetCategoriesQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
    {
        public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await db.Categories.AsNoTracking().ToListAsync(cancellationToken);
            return categories.ToDto();
        }
    }
}
