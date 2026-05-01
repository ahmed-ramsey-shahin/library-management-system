using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Commands.UpdateLibrarianCategories
{
    public sealed class UpdateLibrarianCategoriesCommandHandler(
        IAppDbContext db,
        ILogger<UpdateLibrarianCategoriesCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<UpdateLibrarianCategoriesCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateLibrarianCategoriesCommand request, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .Include(user => user.LibrarianCategories)
                .FirstOrDefaultAsync(user => user.Id == request.LibrarianId, cancellationToken);

            if (user is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("User update aborted. User {UserId} was not found.", request.LibrarianId);
                }

                return ApplicationErrors.UserNotFound;
            }

            if (user.Role != Role.Librarian)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("User update aborted. User {UserId} is not a librarian.", request.LibrarianId);
                }

                return ApplicationErrors.NotLibrarian;
            }

            var uniqueCategoryIds = request.CategoryIds.Distinct().ToList();
            var correctCategories = await db.Categories
                .Where(category => uniqueCategoryIds.Contains(category.Id))
                .CountAsync(cancellationToken);

            if (correctCategories != uniqueCategoryIds.Count)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("User creation aborted. Some category IDs are invalid.");
                }

                return ApplicationErrors.CategoryNotFound;
            }

            var upsertResult = user.UpsertCategories(uniqueCategoryIds);

            if (upsertResult.IsError)
            {
                return upsertResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("user", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Librarian categories updated for {LibrarianId}.", request.LibrarianId);
            }

            return Result.Updated;
        }
    }
}
