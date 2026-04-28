using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Circulation.Policies;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.AcceptBorrowRecord
{
    public sealed class AcceptBorrowRecordCommandHandler(
        IAppDbContext db,
        ILogger<AcceptBorrowRecordCommandHandler> logger,
        HybridCache cache,
        IEnumerable<IBorrowPolicy> borrowPolicies
    ) : IRequestHandler<AcceptBorrowRecordCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(AcceptBorrowRecordCommand request, CancellationToken cancellationToken)
        {
            var borrowRecord = await db.BorrowRecords
                .Include(record => record.BookCopy)
                .Include(record => record.Member)
                .ThenInclude(member => member.BorrowRecords.Where(borrowRecord => borrowRecord.Status != BorrowRecordStatus.Rejected && borrowRecord.Status != BorrowRecordStatus.Returned))
                .Include(record => record.Member)
                .ThenInclude(member => member.Fines.Where(fine => fine.Status == FineStatus.Unpaid))
                .FirstOrDefaultAsync(record => record.Id == request.BorrowRecordId, cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow acceptance aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            if (borrowRecord.Status != BorrowRecordStatus.Waiting)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow acceptance aborted. The record status is {BorrowRecordStatus}.", borrowRecord.Status.ToString());
                }

                return ApplicationErrors.BorrowRecordStatusInvalid;
            }

            var borrowStats = new BorrowStats(
                borrowRecord.Member.BorrowRecords.Count(borrowRecord => borrowRecord.Status == BorrowRecordStatus.Accepted),
                borrowRecord.Member.BorrowRecords.Count(borrowRecord => borrowRecord.Status == BorrowRecordStatus.Late),
                borrowRecord.Member.Fines.Count(fine => fine.Status == FineStatus.Unpaid),
                0
            );

            foreach (var borrowPolicy in borrowPolicies)
            {
                var canBorrow = borrowPolicy.Evaluate(borrowStats);
                if (canBorrow.IsError)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Borrow acceptance aborted. {@Errors}", canBorrow.Errors!);
                    }

                    return canBorrow.Errors!;
                }
            }

            var acceptanceResult = borrowRecord.AcceptBorrowRequest();

            if (acceptanceResult.IsError)
            {
                return acceptanceResult.Errors!;
            }

            var book = await db.Books
                .Include(book => book.BookCopies.Where(copy => copy.Id == borrowRecord.BookCopyId))
                .FirstOrDefaultAsync(book => book.Id == borrowRecord.BookCopy.BookId, cancellationToken);

            var markResult = book!.MarkCopyAsBorrowed(borrowRecord.BookCopyId);

            if (markResult.IsError)
            {
                return markResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync(["borrow-record", "book-copy"], cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Borrow record {BorrowRecordId} accepted.", request.BorrowRecordId);
            }

            return Result.Updated;
       }
    }
}
