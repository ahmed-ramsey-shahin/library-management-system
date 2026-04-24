using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Audiences.Dtos;
using Lms.Application.Features.Audiences.Mappers;
using Lms.Domain.Common.Results;
using Lms.Domain.Metadata;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Audiences.Commands.CreateAudience
{
    public sealed class CreateAudienceCommandHandler(
        IAppDbContext db,
        ILogger<CreateAudienceCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<CreateAudienceCommand, Result<AudienceDto>>
    {
        public async Task<Result<AudienceDto>> Handle(CreateAudienceCommand request, CancellationToken cancellationToken)
        {
            var exists = await db.Audiences.AnyAsync(keyword => string.Equals(keyword.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Audience creation aborted. Audience already exists");
                }

                return ApplicationErrors.AudienceAlreadyExists;
            }

            var keywordCreationResult = Audience.Create(Guid.NewGuid(), request.Name);

            if (keywordCreationResult.IsError)
            {
                return keywordCreationResult.Errors!;
            }

            db.Audiences.Add(keywordCreationResult.Value);
            var keyword = keywordCreationResult.Value.ToDto();
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("keyword", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Audience created successfully. Id: {AudienceId}", keyword.AudienceId);
            }

            return keyword;
        }
    }
}
