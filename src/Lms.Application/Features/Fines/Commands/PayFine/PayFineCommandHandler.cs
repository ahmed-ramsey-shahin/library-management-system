using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Fines.Commands.PayFine
{
    public sealed class PayFineCommandHandler(
        IAppDbContext db,
        ILogger<PayFineCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<PayFineCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(PayFineCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .Include(record => record.Fines.Where(fine => fine.Id == request.FineId))
                .FirstOrDefaultAsync(record => record.Id == request.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Fine payment aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            var paymentResult = borrowRecord.PayFine(request.FineId);

            if (paymentResult.IsError)
            {
                return paymentResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("fine", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Fine {FineId} was paid.", request.FineId);
            }

            return Result.Updated;
        }
    }
}
