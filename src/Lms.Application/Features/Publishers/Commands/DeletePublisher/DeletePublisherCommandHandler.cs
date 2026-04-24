using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Publishers.Commands.DeletePublisher
{
    public sealed class DeletePublisherCommandHandler(
        ILogger<DeletePublisherCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<DeletePublisherCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeletePublisherCommand request, CancellationToken cancellationToken)
        {
            var publisher = await db.Publishers.FirstOrDefaultAsync(publisher => request.PublisherId == publisher.Id, cancellationToken);

            if (publisher is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find publisher with ID {PublisherId}", request.PublisherId);
                }

                return ApplicationErrors.PublisherNotFound;
            }

            var associatedBooks = await db.Books.CountAsync(bg => bg.PublisherId == request.PublisherId, cancellationToken);
            var deletionResult = publisher.Delete(associatedBooks);

            if (deletionResult.IsError)
            {
                return deletionResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("publisher", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Deleted publisher {PublisherId}", request.PublisherId);
            }

            return Result.Deleted;
        }
    }
}
