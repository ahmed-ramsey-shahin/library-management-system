using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Fines.Commands.ProcessLateBorrowRecords
{
    public sealed class ProcessLateBorrowRecordsCommandHandler(
        IAppDbContext db,
        ILogger<ProcessLateBorrowRecordsCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<ProcessLateBorrowRecordsCommand>
    {
        public async Task Handle(ProcessLateBorrowRecordsCommand request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            List<Error> errors = [];
            var lateBorrowRecords = await db.BorrowRecords
                .Where(record => record.Status == BorrowRecordStatus.Late)
                .Include(record => record.Fines)
                .Include(record => record.BookCopy)
                .ThenInclude(copy => copy.Book)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);
            var finesAdded = 0;
            var numberOfErrors = 0;

            foreach (var borrowRecord in lateBorrowRecords)
            {
                var amount = borrowRecord.BookCopy.Book.FinePerDay * (today.DayNumber - borrowRecord.DueDate.DayNumber);
                var fineAdditionResult = borrowRecord.AddFine(
                    id: Guid.NewGuid(),
                    amount: amount,
                    description: $"Daily late return penalty for '{borrowRecord.BookCopy.Book.Title}' at {borrowRecord.BookCopy.Book.FinePerDay:C}/day."
                );

                if (fineAdditionResult.IsError)
                {
                    numberOfErrors++;
                    continue;
                }

                db.Fines.Add(fineAdditionResult.Value);

                try
                {
                    await db.SaveChangesAsync(cancellationToken);
                }
                catch(Exception ex)
                {
                    logger.LogWarning("Exception on BorrowRecord {@Exception}. Skipping.", ex);
                    if (db is DbContext efContext)
                    {
                        efContext.Entry(borrowRecord).State = EntityState.Unchanged;
                        efContext.Entry(fineAdditionResult.Value).State = EntityState.Detached;
                    }
                }

                finesAdded++;
            }

            if (finesAdded > 0)
            {
                await cache.RemoveByTagAsync(["borrow-record", "fine"],cancellationToken);
            }

            if (errors.Count > 0)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Some late borrow records could not be processed. {@Errors}.", errors);
                }
            }
            else if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(typeof(ProcessLateBorrowRecordsCommand).Name + " finished with {NumberOfErrors} errors.", numberOfErrors);
            }
        }
    }
}
