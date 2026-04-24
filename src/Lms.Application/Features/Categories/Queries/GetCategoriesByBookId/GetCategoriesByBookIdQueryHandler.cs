using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Categories.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Categories.Queries.GetCategoriesByBookId
{
    public sealed class GetCategoriesByBookIdQueryHandler(
        IAppDbContext db,
        ILogger<GetCategoriesByBookIdQueryHandler> logger
    ) : IRequestHandler<GetCategoriesByBookIdQuery, Result<List<CategoryDto>>>
    {
        public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesByBookIdQuery request, CancellationToken cancellationToken)
        {
            var bookExists = await db.Books.AnyAsync(book => book.Id == request.BookId, cancellationToken);

            if (!bookExists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find book with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            return await db.Categories
                .Join(db.BookCategories, categorys => categorys.Id, bg => bg.CategoryId, (category, book) => new
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    book.BookId,
                }).Where(category => category.BookId == request.BookId)
                .Select(category => new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.CategoryName,
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
