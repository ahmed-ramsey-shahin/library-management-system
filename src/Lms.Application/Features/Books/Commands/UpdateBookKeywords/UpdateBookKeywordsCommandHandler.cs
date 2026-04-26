using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.UpdateBookKeywords
{
    public sealed class UpdateBookKeywordsCommandHandler(
        IAppDbContext db,
        ILogger<UpdateBookKeywordsCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<UpdateBookKeywordsCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateBookKeywordsCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .Include(book => book.BookKeywords)
                .FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book update aborted. No book was found with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var keywordIds = request.KeywordIds.Distinct().ToList();

            if (keywordIds.Count > 0)
            {
                var existingKeywords = await db.Keywords.CountAsync(
                    a => keywordIds.Contains(a.Id),
                    cancellationToken
                );

                if (existingKeywords != keywordIds.Count)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Book update aborted. One or more keywords are invalid.");
                    }

                    return ApplicationErrors.KeywordNotFound;
                }
            }

            var updateResult = book.UpsertKeywords(keywordIds);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Updated keywords of book {BookId}.", request.BookId);
            }

            return Result.Updated;
        }
    }
}
