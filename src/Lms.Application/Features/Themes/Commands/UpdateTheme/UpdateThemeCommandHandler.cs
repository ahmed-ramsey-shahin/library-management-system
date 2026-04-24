using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Themes.Commands.UpdateTheme
{
    public sealed class UpdateThemeCommandHandler(
        ILogger<UpdateThemeCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<UpdateThemeCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateThemeCommand request, CancellationToken cancellationToken)
        {
            var theme = await db.Themes.FirstOrDefaultAsync(theme => theme.Id == request.ThemeId, cancellationToken);

            if (theme is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find theme with ID {ThemeId}", request.ThemeId);
                }

                return ApplicationErrors.ThemeNotFound;
            }

            var exists = await db.Themes.AnyAsync(theme => string.Equals(theme.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Theme creation aborted. Theme already exists");
                }

                return ApplicationErrors.ThemeAlreadyExists;
            }

            theme.Update(request.Name);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("theme", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Theme {ThemeId} updated successfully.", theme.Id);
            }

            return Result.Updated;
        }
    }
}
