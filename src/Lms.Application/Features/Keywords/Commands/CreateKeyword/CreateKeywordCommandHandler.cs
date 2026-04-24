using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Keywords.Dtos;
using Lms.Application.Features.Keywords.Mappers;
using Lms.Domain.Common.Results;
using Lms.Domain.Metadata;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Keywords.Commands.CreateKeyword
{
    public sealed class CreateKeywordCommandHandler(
        IAppDbContext db,
        ILogger<CreateKeywordCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<CreateKeywordCommand, Result<KeywordDto>>
    {
        public async Task<Result<KeywordDto>> Handle(CreateKeywordCommand request, CancellationToken cancellationToken)
        {
            var exists = await db.Keywords.AnyAsync(keyword => string.Equals(keyword.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Keyword creation aborted. Keyword already exists");
                }

                return ApplicationErrors.KeywordAlreadyExists;
            }

            var keywordCreationResult = Keyword.Create(Guid.NewGuid(), request.Name);

            if (keywordCreationResult.IsError)
            {
                return keywordCreationResult.Errors!;
            }

            db.Keywords.Add(keywordCreationResult.Value);
            var keyword = keywordCreationResult.Value.ToDto();
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("keyword", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Keyword created successfully. Id: {KeywordId}", keyword.KeywordId);
            }

            return keyword;
        }
    }
}
