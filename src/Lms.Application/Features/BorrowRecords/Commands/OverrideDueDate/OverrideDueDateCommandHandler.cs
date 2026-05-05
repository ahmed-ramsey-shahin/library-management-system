using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.OverrideDueDate
{
    public class OverrideDueDateCommandHandler(
        IAppDbContext db,
        ILogger<OverrideDueDateCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<OverrideDueDateCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(OverrideDueDateCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .FirstOrDefaultAsync(record => record.Id == request.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Update borrow record aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            var updateResult = borrowRecord.OverrideDueDate(request.DueDate);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("borrow-record", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Borrow record {BorrowRecordId} due date changed.", request.BorrowRecordId);
            }

            return Result.Updated;
        }
    }
}
