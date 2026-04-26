using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.UpdateBookCategories
{
    public sealed class UpdateBookCategoriesCommandHandler(
        IAppDbContext db,
        ILogger<UpdateBookCategoriesCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<UpdateBookCategoriesCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateBookCategoriesCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .Include(book => book.BookCategories)
                .FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book update aborted. No book was found with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var categoryIds = request.CategoryIds.Distinct().ToList();

            if (categoryIds.Count > 0)
            {
                var existingCategories = await db.Categories.CountAsync(
                    a => categoryIds.Contains(a.Id),
                    cancellationToken
                );

                if (existingCategories != categoryIds.Count)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Book update aborted. One or more categories are invalid.");
                    }

                    return ApplicationErrors.CategoryNotFound;
                }
            }

            var updateResult = book.UpsertCategories(categoryIds);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Updated categories of book {BookId}.", request.BookId);
            }

            return Result.Updated;
        }
    }
}
