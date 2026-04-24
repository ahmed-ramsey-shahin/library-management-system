using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Themes.Commands.DeleteTheme
{
    public sealed class DeleteThemeCommandHandler(
        ILogger<DeleteThemeCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<DeleteThemeCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteThemeCommand request, CancellationToken cancellationToken)
        {
            var theme = await db.Themes.FirstOrDefaultAsync(theme => request.ThemeId == theme.Id, cancellationToken);

            if (theme is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find theme with ID {ThemeId}", request.ThemeId);
                }

                return ApplicationErrors.ThemeNotFound;
            }

            var associatedBooks = await db.BookThemes.CountAsync(bg => bg.ThemeId == request.ThemeId, cancellationToken);
            var deletionResult = theme.Delete(associatedBooks);

            if (deletionResult.IsError)
            {
                return deletionResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("theme", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Deleted theme {ThemeId}", request.ThemeId);
            }

            return Result.Deleted;
        }
    }
}
