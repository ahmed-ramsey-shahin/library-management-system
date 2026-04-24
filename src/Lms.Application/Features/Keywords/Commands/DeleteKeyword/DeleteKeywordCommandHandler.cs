using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Keywords.Commands.DeleteKeyword
{
    public sealed class DeleteKeywordCommandHandler(
        ILogger<DeleteKeywordCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<DeleteKeywordCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteKeywordCommand request, CancellationToken cancellationToken)
        {
            var keyword = await db.Keywords.FirstOrDefaultAsync(keyword => request.KeywordId == keyword.Id, cancellationToken);

            if (keyword is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find keyword with ID {KeywordId}", request.KeywordId);
                }

                return ApplicationErrors.KeywordNotFound;
            }

            var associatedBooks = await db.BookKeywords.CountAsync(bg => bg.KeywordId == request.KeywordId, cancellationToken);
            var deletionResult = keyword.Delete(associatedBooks);

            if (deletionResult.IsError)
            {
                return deletionResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("keyword", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Deleted keyword {KeywordId}", request.KeywordId);
            }

            return Result.Deleted;
        }
    }
}
