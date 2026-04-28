using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Circulation.Policies;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.RenewBorrow
{
    public sealed class RenewBookCommandHandler(
        HybridCache cache,
        ILogger<RenewBookCommandHandler> logger,
        IAppDbContext db,
        IEnumerable<IRenewalPolicy> renewalPolicies
    ) : IRequestHandler<RenewBookCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(RenewBookCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .Include(record => record.Member)
                .ThenInclude(user => user.BorrowRecords.Where(borrowRecord => borrowRecord.Status != BorrowRecordStatus.Rejected && borrowRecord.Status != BorrowRecordStatus.Returned))
                .Include(record => record.Member)
                .ThenInclude(user => user.Fines.Where(fine => fine.Status == FineStatus.Unpaid))
                .Include(record => record.BookCopy)
                .ThenInclude(copy => copy.Book)
                .FirstOrDefaultAsync(borrowRecord => borrowRecord.Id == request.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow renewal aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            if (borrowRecord.Status != BorrowRecordStatus.Accepted)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow renewal aborted. The borrow record is {BorrowRecordStatus}.", borrowRecord.Status.ToString());
                }

                return ApplicationErrors.BorrowRecordStatusInvalid;
            }

            var borrowStats = new BorrowStats(
                borrowRecord.Member.BorrowRecords.Count(borrowRecord => borrowRecord.Status == BorrowRecordStatus.Accepted),
                borrowRecord.Member.BorrowRecords.Count(borrowRecord => borrowRecord.Status == BorrowRecordStatus.Late),
                borrowRecord.Member.Fines.Count(fine => fine.Status == FineStatus.Unpaid),
                borrowRecord.RenewalCount
            );

            foreach (var borrowPolicy in renewalPolicies)
            {
                var canBorrow = borrowPolicy.Evaluate(borrowStats);

                if (canBorrow.IsError)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Borrow renewal aborted. {@Errors}", canBorrow.Errors!);
                    }

                    return canBorrow.Errors!;
                }
            }

            if (request.DueDate <= borrowRecord.DueDate)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow renewal aborted. The new due date is before the old due date.");
                }

                return ApplicationErrors.NewDueDateInvalid;
            }

            var renewalResult = borrowRecord.Renew(
                (request.DueDate.DayNumber - borrowRecord.DueDate.DayNumber) * borrowRecord.BookCopy.Book.BorrowPricePerDay,
                request.DueDate
            );

            if (renewalResult.IsError)
            {
                return renewalResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("borrow-record", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Borrow record {BorrowRecordId} was renewed.", request.BorrowRecordId);
            }

            return Result.Updated;
        }
    }
}
