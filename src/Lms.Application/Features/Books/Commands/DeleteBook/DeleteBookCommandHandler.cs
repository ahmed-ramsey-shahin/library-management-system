using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.DeleteBook
{
    public sealed class DeleteBookCommandHandler(
        IAppDbContext db,
        HybridCache cache,
        ILogger<DeleteBookCommandHandler> logger
    ) : IRequestHandler<DeleteBookCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books.FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book deletion aborted. No book was found with ID {BookId}.", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var totalAssociatedCopies = await db.BookCopies.CountAsync(copy => copy.BookId == request.BookId, cancellationToken);
            var deletionResult = book.Delete(totalAssociatedCopies);

            if (deletionResult.IsError)
            {
                return deletionResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Book {BookId} was deleted.", request.BookId);
            }

            return Result.Deleted;
        }
    }
}
