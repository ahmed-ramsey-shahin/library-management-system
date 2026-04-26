using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.DeleteBookCopy
{
    public sealed class DeleteBookCopyCommandHandler(
        ILogger<DeleteBookCopyCommandHandler> logger,
        HybridCache cache,
        IAppDbContext db
    ) : IRequestHandler<DeleteBookCopyCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteBookCopyCommand request, CancellationToken cancellationToken)
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

            var removalResult = book.RemoveCopy(request.CopyId);

            if (removalResult.IsError)
            {
                return removalResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book-copy", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Book copy {CopyId} was deleted.", request.CopyId);
            }

            return Result.Deleted;
        }
    }
}
