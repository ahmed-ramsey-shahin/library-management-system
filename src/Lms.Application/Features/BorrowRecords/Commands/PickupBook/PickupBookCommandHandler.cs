using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.PickupBook
{
    public sealed class PickupBookCommandHandler(
        IAppDbContext db,
        ILogger<PickupBookCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<PickupBookCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(PickupBookCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords.FirstOrDefaultAsync(record => record.Id == request.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("No borrow record was found with id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            var pickupResult = borrowRecord.Pickup();

            if (pickupResult.IsError)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError("Could no pick up book {BorrowRecordId}. {@Errors}.", request.BorrowRecordId, pickupResult.Errors!);
                }

                return pickupResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync(["borrow-record"], cancellationToken);
            return Result.Updated;
        }
    }
}
