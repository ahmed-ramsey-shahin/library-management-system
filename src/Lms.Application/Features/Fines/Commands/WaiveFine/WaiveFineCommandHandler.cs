using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Fines.Commands.WaiveFine
{
    public sealed class WaiveFineCommandHandler(
        IAppDbContext db,
        ILogger<WaiveFineCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<WaiveFineCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(WaiveFineCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .Include(record => record.Fines.Where(fine => fine.Id == request.FineId))
                .FirstOrDefaultAsync(record => record.Id == request.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Fine waive aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            var result = borrowRecord.MarkFineWaived(request.FineId);

            if (result.IsError)
            {
                return result.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("fine", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Fine {FineId} was waived.", request.FineId);
            }

            return Result.Updated;
        }
    }
}
