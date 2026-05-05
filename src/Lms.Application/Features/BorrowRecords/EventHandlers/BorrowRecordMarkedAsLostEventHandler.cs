using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Books.Commands.UpdateBookCopyStatus;
using Lms.Application.Features.Fines.Commands.IssueFine;
using Lms.Domain.Catalog;
using Lms.Domain.Circulation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.EventHandlers
{
    public class BorrowRecordMarkedAsLostEventHandler(
        ISender sender,
        ILogger<BorrowRecordMarkedAsLostEventHandler> logger,
        IAppDbContext db
    ) : INotificationHandler<BorrowRecordMarkedAsLostEvent>
    {
        public async Task Handle(BorrowRecordMarkedAsLostEvent notification, CancellationToken cancellationToken)
        {
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("-- Borrow record {BorrowRecordId} marked as lost event started. --", notification.BorrowRecordId);

            var borrowRecord = await db.BorrowRecords
                .AsNoTracking()
                .Include(record => record.BookCopy)
                .ThenInclude(copy => copy.Book)
                .FirstOrDefaultAsync(record => record.Id == notification.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                logger.LogError("Event could not be handled. Borrow record {BorrowRecordId} was not found.", notification.BorrowRecordId);
                return;
            }

            logger.LogInformation("Marking the book copy as lost ...");
            await sender.Send(new UpdateBookCopyStatusCommand(
                BookId: borrowRecord.BookCopy.BookId,
                CopyId: borrowRecord.BookCopyId,
                Status: BookCopyStatus.Lost
            ), cancellationToken);
            await sender.Send(new IssueFineCommand(
                BorrowRecordId: notification.BorrowRecordId,
                Amount: borrowRecord.BookCopy.Book.LostFee,
                Description: $"Lost book replacement fee for '{borrowRecord.BookCopy.Book.Title}'. (Flat rate: {borrowRecord.BookCopy.Book.LostFee:C}).",
                FineDate: DateTimeOffset.UtcNow,
                IdempotencyKey: $"LostBookFine-{notification.BorrowRecordId}"
            ), cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("-- Borrow record {BorrowRecordId} marked as lost event handled. --", notification.BorrowRecordId);
        }
    }
}
