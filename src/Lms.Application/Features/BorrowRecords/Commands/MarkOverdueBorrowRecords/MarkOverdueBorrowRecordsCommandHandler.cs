using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.MarkOverdueBorrowRecords
{
    public sealed class MarkOverdueBorrowRecordsCommandHandler(
        IAppDbContext db,
        ILogger<MarkOverdueBorrowRecordsCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<MarkOverdueBorrowRecordsCommand>
    {
        public async Task Handle(MarkOverdueBorrowRecordsCommand request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            List<Error> errors = [];
            var lateBorrowRecords = await db.BorrowRecords
                .Where(record => today > record.DueDate && record.Status == BorrowRecordStatus.Accepted)
                .ToListAsync(cancellationToken);

            foreach (var borrowRecord in lateBorrowRecords)
            {
                var markResult = borrowRecord.MarkAsLate();

                if (markResult.IsError)
                {
                    if (logger.IsEnabled(LogLevel.Error))
                    {
                        logger.LogError("Could not mark {BorrowRecordId} as late {@Errors}", borrowRecord.Id, markResult.Errors);
                    }

                    errors.AddRange(markResult.Errors!);
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("borrow-record", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Processed late borrow records with {NumberOfErrors} Errors.", errors.Count);
            }
        }
    }
}
