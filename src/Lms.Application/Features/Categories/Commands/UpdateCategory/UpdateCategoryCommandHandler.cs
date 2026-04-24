using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Categories.Commands.UpdateCategory
{
    public sealed class UpdateCategoryCommandHandler(
        ILogger<UpdateCategoryCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<UpdateCategoryCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await db.Categories.FirstOrDefaultAsync(category => category.Id == request.CategoryId, cancellationToken);

            if (category is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find category with ID {CategoryId}", request.CategoryId);
                }

                return ApplicationErrors.CategoryNotFound;
            }

            var exists = await db.Categories.AnyAsync(category => string.Equals(category.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Category creation aborted. Category already exists");
                }

                return ApplicationErrors.CategoryAlreadyExists;
            }

            category.Update(request.Name);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("category", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Category {CategoryId} updated successfully.", category.Id);
            }

            return Result.Updated;
        }
    }
}
