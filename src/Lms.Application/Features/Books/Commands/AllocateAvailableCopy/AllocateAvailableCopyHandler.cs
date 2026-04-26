using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.AllocateAvailableCopy
{
    public sealed class AllocateAvailableCopyHandler(
        IAppDbContext db,
        ILogger<AllocateAvailableCopyHandler> logger,
        HybridCache cache
    ) : IRequestHandler<AllocateAvailableCopyCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AllocateAvailableCopyCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .Include(book => book.BookCopies.Where(copy => copy.State == BookCopyState.Available))
                .FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book copy allocation aborted. No book was found with ID {BookId}.", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var allocationResult = book.AllocateAvailableCopy();

            if (allocationResult.IsError)
            {
                return allocationResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync(["book", "book-copy"], cancellationToken);
            var copy = allocationResult.Value;

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Book copy {CopyId} was allocated from book {BookId}.", copy.Id, request.BookId);
            }

            return copy.Id;
        }
    }
}
