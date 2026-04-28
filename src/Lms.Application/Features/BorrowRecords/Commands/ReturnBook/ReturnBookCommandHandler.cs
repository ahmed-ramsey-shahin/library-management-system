using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.ReturnBook
{
    public sealed class ReturnBookCommandHandler(
        IAppDbContext db,
        HybridCache cache,
        ILogger<ReturnBookCommandHandler> logger
    ) : IRequestHandler<ReturnBookCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .Include(record => record.BookCopy)
                .FirstOrDefaultAsync(record => record.Id == request.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow return aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            if (borrowRecord.Status != BorrowRecordStatus.Accepted && borrowRecord.Status != BorrowRecordStatus.Late)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow return aborted. The record status is {BorrowRecordStatus}.", borrowRecord.Status.ToString());
                }

                return ApplicationErrors.BorrowRecordStatusInvalid;
            }

            var returnResult = borrowRecord.Return();

            if (returnResult.IsError)
            {
                return returnResult.Errors!;
            }

            var book = await db.Books
                .Include(book => book.BookCopies.Where(copy => copy.Id == borrowRecord.BookCopyId))
                .FirstOrDefaultAsync(book => book.Id == borrowRecord.BookCopy.BookId, cancellationToken);
            var markAvailableResult = book!.MarkCopyAsAvailable(borrowRecord.BookCopyId);

            if (markAvailableResult.IsError)
            {
                return markAvailableResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync(["borrow-record", "book-copy"], cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("The book copy {BookCopyId} was returned.", borrowRecord.BookCopyId);
            }

            return Result.Updated;
        }
    }
}
