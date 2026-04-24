using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Publishers.Commands.UpdatePublisher
{
    public sealed class UpdatePublisherCommandHandler(
        ILogger<UpdatePublisherCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<UpdatePublisherCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdatePublisherCommand request, CancellationToken cancellationToken)
        {
            var publisher = await db.Publishers.FirstOrDefaultAsync(publisher => publisher.Id == request.PublisherId, cancellationToken);

            if (publisher is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find publisher with ID {PublisherId}", request.PublisherId);
                }

                return ApplicationErrors.PublisherNotFound;
            }

            var exists = await db.Publishers.AnyAsync(publisher => string.Equals(publisher.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Publisher creation aborted. Publisher already exists");
                }

                return ApplicationErrors.PublisherAlreadyExists;
            }

            publisher.Update(request.Name);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("publisher", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Publisher {PublisherId} updated successfully.", publisher.Id);
            }

            return Result.Updated;
        }
    }
}
