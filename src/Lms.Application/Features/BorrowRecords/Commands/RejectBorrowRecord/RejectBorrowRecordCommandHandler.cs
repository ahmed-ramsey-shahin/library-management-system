using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.RejectBorrowRecord
{
    public sealed class RejectBorrowRecordCommandHandler(
        IAppDbContext db,
        ILogger<RejectBorrowRecordCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<RejectBorrowRecordCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(RejectBorrowRecordCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .Include(record => record.BookCopy)
                .FirstOrDefaultAsync(record => record.Id == request.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow rejection aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            if (borrowRecord.Status != BorrowRecordStatus.Waiting)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow rejection aborted. The record status is {BorrowRecordStatus}.", borrowRecord.Status.ToString());
                }

                return ApplicationErrors.BorrowRecordStatusInvalid;
            }

            var rejectionResult = borrowRecord.RejectBorrowRequest();

            if (rejectionResult.IsError)
            {
                return rejectionResult.Errors!;
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
                logger.LogInformation("Borrow record {BorrowRecordId} rejected.", request.BorrowRecordId);
            }

            return Result.Updated;
        }
    }
}
