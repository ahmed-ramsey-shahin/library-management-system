using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Keywords.Commands.UpdateKeyword
{
    public sealed class UpdateKeywordCommandHandler(
        ILogger<UpdateKeywordCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<UpdateKeywordCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateKeywordCommand request, CancellationToken cancellationToken)
        {
            var keyword = await db.Keywords.FirstOrDefaultAsync(keyword => keyword.Id == request.KeywordId, cancellationToken);

            if (keyword is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find keyword with ID {KeywordId}", request.KeywordId);
                }

                return ApplicationErrors.KeywordNotFound;
            }

            var exists = await db.Keywords.AnyAsync(keyword => string.Equals(keyword.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Keyword creation aborted. Keyword already exists");
                }

                return ApplicationErrors.KeywordAlreadyExists;
            }

            keyword.Update(request.Name);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("keyword", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Keyword {KeywordId} updated successfully.", keyword.Id);
            }

            return Result.Updated;
        }
    }
}
