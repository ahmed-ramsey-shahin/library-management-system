using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Themes.Dtos;
using Lms.Application.Features.Themes.Mappers;
using Lms.Domain.Common.Results;
using Lms.Domain.Metadata;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Themes.Commands.CreateTheme
{
    public sealed class CreateThemeCommandHandler(
        IAppDbContext db,
        ILogger<CreateThemeCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<CreateThemeCommand, Result<ThemeDto>>
    {
        public async Task<Result<ThemeDto>> Handle(CreateThemeCommand request, CancellationToken cancellationToken)
        {
            var exists = await db.Themes.AnyAsync(theme => string.Equals(theme.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Theme creation aborted. Theme already exists");
                }

                return ApplicationErrors.ThemeAlreadyExists;
            }

            var themeCreationResult = Theme.Create(Guid.NewGuid(), request.Name);

            if (themeCreationResult.IsError)
            {
                return themeCreationResult.Errors!;
            }

            db.Themes.Add(themeCreationResult.Value);
            var theme = themeCreationResult.Value.ToDto();
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("theme", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Theme created successfully. Id: {ThemeId}", theme.ThemeId);
            }

            return theme;
        }
    }
}
