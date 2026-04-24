using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Audiences.Commands.UpdateAudience
{
    public sealed class UpdateAudienceCommandHandler(
        ILogger<UpdateAudienceCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<UpdateAudienceCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateAudienceCommand request, CancellationToken cancellationToken)
        {
            var keyword = await db.Audiences.FirstOrDefaultAsync(keyword => keyword.Id == request.AudienceId, cancellationToken);

            if (keyword is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find keyword with ID {AudienceId}", request.AudienceId);
                }

                return ApplicationErrors.AudienceNotFound;
            }

            var exists = await db.Audiences.AnyAsync(keyword => string.Equals(keyword.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Audience creation aborted. Audience already exists");
                }

                return ApplicationErrors.AudienceAlreadyExists;
            }

            keyword.Update(request.Name);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("keyword", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Audience {AudienceId} updated successfully.", keyword.Id);
            }

            return Result.Updated;
        }
    }
}
