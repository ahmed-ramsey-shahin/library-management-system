using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Categories.Commands.DeleteCategory
{
    public sealed class DeleteCategoryCommandHandler(
        ILogger<DeleteCategoryCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<DeleteCategoryCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await db.Categories.FirstOrDefaultAsync(category => request.CategoryId == category.Id, cancellationToken);

            if (category is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find category with ID {CategoryId}", request.CategoryId);
                }

                return ApplicationErrors.CategoryNotFound;
            }

            var associatedBooks = await db.BookCategories.CountAsync(bg => bg.CategoryId == request.CategoryId, cancellationToken);
            var associatedLibrarians = await db.LibrarianCategories.CountAsync(bg => bg.CategoryId == request.CategoryId, cancellationToken);
            var deletionResult = category.Delete(associatedBooks, associatedLibrarians);

            if (deletionResult.IsError)
            {
                return deletionResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("category", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Deleted category {CategoryId}", request.CategoryId);
            }

            return Result.Deleted;
        }
    }
}
