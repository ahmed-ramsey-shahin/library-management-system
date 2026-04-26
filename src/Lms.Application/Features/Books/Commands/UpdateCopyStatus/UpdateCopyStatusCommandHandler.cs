using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.UpdateCopyStatus
{
    public sealed class UpdateCopyStatusCommandHandler(
        IAppDbContext db,
        ILogger<UpdateCopyStatusCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<UpdateCopyStatusCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateCopyStatusCommand request, CancellationToken cancellationToken)
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

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync(["book", "book-copy"], cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("The status of copy {CopyId} was changed.", request.CopyId);
            }

            return Result.Updated;
        }
    }
}
