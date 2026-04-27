using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.MarkBookCopyAsAvailable
{
    public sealed class MarkBookCopyAsAvailableCommandHandler(
        IAppDbContext db,
        ILogger<MarkBookCopyAsAvailableCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<MarkBookCopyAsAvailableCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(MarkBookCopyAsAvailableCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .Include(book => book.BookCopies.Where(copy => copy.Id == request.CopyId))
                .FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book copy update aborted. No book was found with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var updateResult = book.MarkCopyAsAvailable(request.CopyId);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            var copy = book.BookCopies.FirstOrDefault(copy => copy.Id == request.CopyId);
            db.SetOriginalVersion(copy!, request.Version);

            try {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch(DbUpdateConcurrencyException)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError("Book copy update aborted beacuse of a concurrency conflict.");
                }

                return ApplicationErrors.ConcurrencyConflict;
            }

            await cache.RemoveByTagAsync("book-copy", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Marked book copy {CopyId} as available.", request.CopyId);
            }

            return Result.Updated;
        }
    }
}
