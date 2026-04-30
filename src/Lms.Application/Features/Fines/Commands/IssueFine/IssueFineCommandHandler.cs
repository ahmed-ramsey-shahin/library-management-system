using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Fines.Commands.IssueFine
{
    public sealed class IssueFineCommandHandler(
        IAppDbContext db,
        ILogger<IssueFineCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<IssueFineCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(IssueFineCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .Where(record => record.Id == request.BorrowRecordId)
                .Include(record => record.Fines)
                .FirstOrDefaultAsync(cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Fine creation aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            var fineResult = borrowRecord.AddFine(
                Guid.NewGuid(),
                request.Amount,
                request.Description,
                DateTimeOffset.UtcNow
            );

            if (fineResult.IsError)
            {
                return fineResult.Errors!;
            }

            var fine = fineResult.Value;
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("fine", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Fine {FineId} for borrow record {BorrowRecordId}.", fine.Id, request.BorrowRecordId);
            }

            return fine.Id;
        }
    }
}
