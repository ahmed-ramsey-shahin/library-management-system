using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.UpdateBookAuthors
{
    public sealed class UpdateBookAuthorsCommandHandler(
        IAppDbContext db,
        ILogger<UpdateBookAuthorsCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<UpdateBookAuthorsCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateBookAuthorsCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .Include(book => book.BookAuthors)
                .FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book update aborted. No book was found with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var authorIds = request.AuthorIds.Distinct().ToList();

            if (authorIds.Count > 0)
            {
                var existingAuthors = await db.Authors.CountAsync(
                    a => authorIds.Contains(a.Id),
                    cancellationToken
                );

                if (existingAuthors != authorIds.Count)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Book update aborted. One or more authors are invalid.");
                    }

                    return ApplicationErrors.AuthorNotFound;
                }
            }

            var updateResult = book.UpsertAuthors(authorIds);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Updated authors of book {BookId}.", request.BookId);
            }

            return Result.Updated;
        }
    }
}
