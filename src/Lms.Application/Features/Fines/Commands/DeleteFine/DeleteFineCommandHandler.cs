using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Fines.Commands.DeleteFine
{
    public sealed class DeleteFineCommandHandler(
        IAppDbContext db,
        ILogger<DeleteFineCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<DeleteFineCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteFineCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .Where(record => record.Id == request.BorrowRecordId)
                .Include(record => record.Fines.Where(fine => fine.Id == request.FineId))
                .FirstOrDefaultAsync(cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Fine deletion aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            var result = borrowRecord.RemoveFine(request.FineId);

            if (result.IsError)
            {
                return result.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("fine", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Fine {FineId} was deleted.", request.FineId);
            }

            return Result.Deleted;
        }
    }
}
