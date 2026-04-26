using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.UpdateBookAudiences
{
    public sealed class UpdateBookAudiencesCommandHandler(
        IAppDbContext db,
        ILogger<UpdateBookAudiencesCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<UpdateBookAudiencesCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateBookAudiencesCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .Include(book => book.BookAudiences)
                .FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book update aborted. No book was found with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var audienceIds = request.AudienceIds.Distinct().ToList();

            if (audienceIds.Count > 0)
            {
                var existingAudiences = await db.Audiences.CountAsync(
                    a => audienceIds.Contains(a.Id),
                    cancellationToken
                );

                if (existingAudiences != audienceIds.Count)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Book update aborted. One or more audiences are invalid.");
                    }

                    return ApplicationErrors.AudienceNotFound;
                }
            }

            var updateResult = book.UpsertAudiences(audienceIds);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Updated audiences of book {BookId}.", request.BookId);
            }

            return Result.Updated;
        }
    }
}
