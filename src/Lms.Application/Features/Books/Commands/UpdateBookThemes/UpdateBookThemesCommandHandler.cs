using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.UpdateBookThemes
{
    public sealed class UpdateBookThemesCommandHandler(
        IAppDbContext db,
        ILogger<UpdateBookThemesCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<UpdateBookThemesCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateBookThemesCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .Include(book => book.BookThemes)
                .FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book update aborted. No book was found with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var themeIds = request.ThemeIds.Distinct().ToList();

            if (themeIds.Count > 0)
            {
                var existingThemes = await db.Themes.CountAsync(
                    a => themeIds.Contains(a.Id),
                    cancellationToken
                );

                if (existingThemes != themeIds.Count)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Book update aborted. One or more themes are invalid.");
                    }

                    return ApplicationErrors.ThemeNotFound;
                }
            }

            var updateResult = book.UpsertThemes(themeIds);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Updated themes of book {BookId}.", request.BookId);
            }

            return Result.Updated;
        }
    }
}
