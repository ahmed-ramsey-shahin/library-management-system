using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Publishers.Dtos;
using Lms.Application.Features.Publishers.Mappers;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Publishers.Commands.CreatePublisher
{
    public sealed class CreatePublisherCommandHandler(
        IAppDbContext db,
        ILogger<CreatePublisherCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<CreatePublisherCommand, Result<PublisherDto>>
    {
        public async Task<Result<PublisherDto>> Handle(CreatePublisherCommand request, CancellationToken cancellationToken)
        {
            var exists = await db.Publishers.AnyAsync(publisher => string.Equals(publisher.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Publisher creation aborted. Publisher already exists");
                }

                return ApplicationErrors.PublisherAlreadyExists;
            }

            var publisherCreationResult = Publisher.Create(Guid.NewGuid(), request.Name);

            if (publisherCreationResult.IsError)
            {
                return publisherCreationResult.Errors!;
            }

            db.Publishers.Add(publisherCreationResult.Value);
            var publisher = publisherCreationResult.Value.ToDto();
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("publisher", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Publisher created successfully. Id: {PublisherId}", publisher.PublisherId);
            }

            return publisher;
        }
    }
}
