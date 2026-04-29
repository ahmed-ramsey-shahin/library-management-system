using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.CancelBorrowRecord
{
    public sealed class CancelBorrowRecordCommandHandler(
        IAppDbContext db,
        HybridCache cache,
        ILogger<CancelBorrowRecordCommandHandler> logger
    ) : IRequestHandler<CancelBorrowRecordCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(CancelBorrowRecordCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .Include(record => record.BookCopy)
                .FirstOrDefaultAsync(record => record.Id == request.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow record cancellation aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            var cancellationResult = borrowRecord.Cancel();

            if (cancellationResult.IsError)
            {
                return cancellationResult.Errors!;
            }

            var book = await db.Books
                .Include(book => book.BookCopies.Where(copy => copy.Id == borrowRecord.BookCopyId))
                .FirstOrDefaultAsync(book => book.Id == borrowRecord.BookCopy.BookId, cancellationToken);
            var markResult = book!.MarkCopyAsAvailable(borrowRecord.BookCopyId);

            if (markResult.IsError)
            {
                return markResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync(["borrow-record", "book-copy"], cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Borrow record {BorrowRecordId} was canceled.", request.BorrowRecordId);
            }

            return Result.Updated;
        }
    }
}
