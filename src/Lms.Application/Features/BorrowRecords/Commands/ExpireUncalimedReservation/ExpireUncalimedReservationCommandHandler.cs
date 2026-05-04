using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.ExpireUncalimedReservation
{
    public sealed class ExpireUncalimedReservationCommandHandler(
        ILogger<ExpireUncalimedReservationCommandHandler> logger,
        HybridCache cache,
        IAppDbContext db
    ) : IRequestHandler<ExpireUncalimedReservationCommand>
    {
        public async Task Handle(ExpireUncalimedReservationCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Expiring uncliamed reservations ...");
            List<Error> errors = [];
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var cutoffDate = today.AddDays(-3);
            var borrowRecords = await db.BorrowRecords
                .Include(record => record.BookCopy)
                .Where(record => record.Status == BorrowRecordStatus.Accepted && record.PickupDeadline <= cutoffDate && !record.PickedUp)
                .ToListAsync(cancellationToken);

            if (borrowRecords.Count == 0)
            {
                return;
            }

            var bookIds = borrowRecords.Select(record => record.BookCopy.BookId).Distinct();
            var booksDict = await db.Books
                .Include(book => book.BookCopies)
                .Where(book => bookIds.Contains(book.Id))
                .ToDictionaryAsync(book => book.Id, cancellationToken);

            foreach (var borrowRecord in borrowRecords)
            {
                var updateResult = borrowRecord.RejectBorrowRequest();

                if (updateResult.IsError)
                {
                    logger.LogError("Borrow record {BorrowRecordId} could not be rejected. {@Errors}.", updateResult.Errors);
                    continue;
                }

                var book = booksDict.GetValueOrDefault(borrowRecord.BookCopy.BookId)!;
                var markAvailableResult = book!.MarkCopyAsAvailable(borrowRecord.BookCopyId);

                if (markAvailableResult.IsError)
                {
                    logger.LogError("Book copy {BookCopyId} could not be marked as avilable. {@Errors}", markAvailableResult.Errors);
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync(["book-copy", "borrow-record"], cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Uncliamed reservations expired.");
            }
        }
    }
}
