using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Fines.Commands.ChangeFineAmount
{
    public sealed class ChangeFineAmountCommandHandler(
        IAppDbContext db,
        ILogger<ChangeFineAmountCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<ChangeFineAmountCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(ChangeFineAmountCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .Where(record => record.Id == request.BorrowRecordId)
                .Include(record => record.Fines.Where(fine => fine.Id == request.FineId))
                .FirstOrDefaultAsync(cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Fine update aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            var result = borrowRecord.ChangeFineAmount(request.FineId, request.Amount);

            if (result.IsError)
            {
                return result.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("fine", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Changed fine {FineId} amount.", request.FineId);
            }

            return Result.Updated;
        }
    }
}
