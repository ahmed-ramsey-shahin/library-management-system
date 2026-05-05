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
            List<Error> errors = [];
            var lateBorrowRecords = await db.BorrowRecords
                .Where(record => record.Status == BorrowRecordStatus.Late)
                .Include(record => record.Fines)
                .Include(record => record.BookCopy)
                .ThenInclude(copy => copy.Book)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);
            var finesAdded = 0;

            foreach (var borrowRecord in lateBorrowRecords)
            {
                var fineAdditionResult = borrowRecord.AddFine(
                    id: Guid.NewGuid(),
                    amount: borrowRecord.BookCopy.Book.FinePerDay,
                    description: $"Daily late return penalty for '{borrowRecord.BookCopy.Book.Title}' at {borrowRecord.BookCopy.Book.FinePerDay:C}/day."
                );

                if (fineAdditionResult.IsError)
                {
                    errors.AddRange(fineAdditionResult.Errors!);
                }
                else
                {
                    finesAdded++;
                }
            }

            if (finesAdded > 0)
            {
                await db.SaveChangesAsync(cancellationToken);
                await cache.RemoveByTagAsync(["borrow-record", "fine"],cancellationToken);
            }

            if (errors.Count > 0 && logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("Some late borrow records could not be processed. {@Errors}.", errors);
            }
        }
    }
}
