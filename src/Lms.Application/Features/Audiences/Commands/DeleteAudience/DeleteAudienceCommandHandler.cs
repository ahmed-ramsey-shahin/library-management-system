using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Audiences.Commands.DeleteAudience
{
    public sealed class DeleteAudienceCommandHandler(
        ILogger<DeleteAudienceCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<DeleteAudienceCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteAudienceCommand request, CancellationToken cancellationToken)
        {
            var audience = await db.Audiences.FirstOrDefaultAsync(audience => request.AudienceId == audience.Id, cancellationToken);

            if (audience is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find audience with ID {AudienceId}", request.AudienceId);
                }

                return ApplicationErrors.AudienceNotFound;
            }

            var associatedBooks = await db.BookAudiences.CountAsync(bg => bg.AudienceId == request.AudienceId, cancellationToken);
            var deletionResult = audience.Delete(associatedBooks);

            if (deletionResult.IsError)
            {
                return deletionResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("audience", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Deleted audience {AudienceId}", request.AudienceId);
            }

            return Result.Deleted;
        }
    }
}
