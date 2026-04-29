using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.MarkRecordAsLate
{
    public sealed class MarkRecordAsLateCommandHandler(
        IAppDbContext db,
        ILogger<MarkRecordAsLateCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<MarkRecordAsLateCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(MarkRecordAsLateCommand request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var borrowRecord = await db.BorrowRecords
                .FirstOrDefaultAsync(record => record.Id == request.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Mark as late aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            if (borrowRecord.Status != BorrowRecordStatus.Accepted)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Mark as late aborted. The record status is {BorrowRecordStatus}.", borrowRecord.Status.ToString());
                }

                return ApplicationErrors.BorrowRecordStatusInvalid;
            }

            if (today <= borrowRecord.DueDate)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Mark as late aborted. The borrow record specified ({BorrowRecordId}) is not late.", request.BorrowRecordId);
                }

                return ApplicationErrors.NotLate;
            }

            var markResult = borrowRecord.MarkAsLate();

            if (markResult.IsError)
            {
                return markResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("borrow-record", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Borrow record {BorrowRecordId} marked as late.", request.BorrowRecordId);
            }

            return Result.Updated;
        }
    }
}
