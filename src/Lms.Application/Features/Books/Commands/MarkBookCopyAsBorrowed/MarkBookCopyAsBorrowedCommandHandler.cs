using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.MarkBookCopyAsBorrowed
{
    public sealed class MarkBookCopyAsBorrowedCommandHandler(
        IAppDbContext db,
        ILogger<MarkBookCopyAsBorrowedCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<MarkBookCopyAsBorrowedCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(MarkBookCopyAsBorrowedCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .Include(book => book.BookCopies.Where(copy => copy.Id == request.CopyId))
                .FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book copy deletion aborted. No book was found with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var updateResult = book.MarkCopyAsBorrowed(request.CopyId);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book-copy", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Marked book copy {CopyId} as borrowed.", request.CopyId);
            }

            return Result.Updated;
        }
    }
}
