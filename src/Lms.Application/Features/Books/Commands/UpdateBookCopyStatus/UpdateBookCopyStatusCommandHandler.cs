using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.UpdateBookCopyStatus
{
    public sealed class UpdateBookCopyStatusCommandHandler(
        IAppDbContext db,
        ILogger<UpdateBookCopyStatusCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<UpdateBookCopyStatusCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateBookCopyStatusCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .Include(book => book.BookCopies.Where(copy => copy.Id == request.CopyId))
                .FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if(book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book copy allocation aborted. No book was found with ID {BookId}.", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var changeResult = book.ChangeCopyStatus(request.CopyId, request.Status);

            if (changeResult.IsError)
            {
                return changeResult.Errors!;
            }

            var bookCopy = book.BookCopies.FirstOrDefault(copy => copy.Id == request.CopyId);
            db.SetOriginalVersion(bookCopy!, request.Version);

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch(DbUpdateConcurrencyException)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError("Book copy update aborted because of a concurrency conflict.");
                }

                return ApplicationErrors.ConcurrencyConflict;
            }

            await cache.RemoveByTagAsync(["book", "book-copy"], cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("The status of copy {CopyId} was changed.", request.CopyId);
            }

            return Result.Updated;
        }
    }
}
