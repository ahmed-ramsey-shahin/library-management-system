using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.ProcessLateBorrowRecords
{
    public sealed class ProcessLateBorrowRecordsCommandHandler(
        IAppDbContext db,
        ILogger<ProcessLateBorrowRecordsCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<ProcessLateBorrowRecordsCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(ProcessLateBorrowRecordsCommand request, CancellationToken cancellationToken)
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

            if (errors.Count > 0)
            {
                return errors;
            }

            return Result.Updated;
        }
    }
}
