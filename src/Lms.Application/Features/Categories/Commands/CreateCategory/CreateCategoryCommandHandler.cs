using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Categories.Dtos;
using Lms.Application.Features.Categories.Mappers;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Categories.Commands.CreateCategory
{
    public sealed class CreateCategoryCommandHandler(
        IAppDbContext db,
        ILogger<CreateCategoryCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
    {
        public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var exists = await db.Categories.AnyAsync(category => string.Equals(category.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Category creation aborted. Category already exists");
                }

                return ApplicationErrors.CategoryAlreadyExists;
            }

            var categoryCreationResult = Category.Create(Guid.NewGuid(), request.Name);

            if (categoryCreationResult.IsError)
            {
                return categoryCreationResult.Errors!;
            }

            db.Categories.Add(categoryCreationResult.Value);
            var category = categoryCreationResult.Value.ToDto();
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("category", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Category created successfully. Id: {CategoryId}", category.CategoryId);
            }

            return category;
        }
    }
}
