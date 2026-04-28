using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Circulation.Policies;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.BorrowBook
{
    public sealed class BorrowBookCommandHandler(
        IAppDbContext db,
        ILogger<BorrowBookCommandHandler> logger,
        HybridCache cache,
        IEnumerable<IBorrowPolicy> borrowPolicies
    ) : IRequestHandler<BorrowBookCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var member = await db.Users
                .AsNoTracking()
                .Include(user => user.BorrowRecords.Where(borrowRecord => borrowRecord.Status != BorrowRecordStatus.Rejected && borrowRecord.Status != BorrowRecordStatus.Returned))
                .ThenInclude(borrowRecord => borrowRecord.BookCopy)
                .ThenInclude(copy => copy.Book)
                .Include(user => user.Fines.Where(fine => fine.Status == FineStatus.Unpaid))
                .AsSplitQuery()
                .FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);

            if (member is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Operation aborted. No user was found with ID {UserId}.", request.UserId);
                }

                return ApplicationErrors.UserNotFound;
            }

            if (member.Role != Role.Member)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Operation aborted. The specified user ({UserId}) is not a member.", request.UserId);
                }

                return ApplicationErrors.UserNotMember;
            }

            var borrowStats = new BorrowStats(
                member.BorrowRecords.Count(borrowRecord => borrowRecord.Status == BorrowRecordStatus.Accepted),
                member.BorrowRecords.Count(borrowRecord => borrowRecord.Status == BorrowRecordStatus.Late),
                member.Fines.Count(fine => fine.Status == FineStatus.Unpaid),
                0
            );

            foreach (var borrowPolicy in borrowPolicies)
            {
                var canBorrow = borrowPolicy.Evaluate(borrowStats);
                if (canBorrow.IsError)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Operation aborted. {@Errors}", canBorrow.Errors!);
                    }

                    return canBorrow.Errors!;
                }
            }

            var book = await db.Books
                .Include(book => book.BookCopies)
                .Where(book => book.Id == request.BookId)
                .FirstOrDefaultAsync(cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow request creation aborted. No book was found with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var bookAlreadyBorrowed = member.BorrowRecords.Any(
                borrowRecord => borrowRecord.BookCopy.Book.Isbn == book.Isbn &&
                borrowRecord.Status != BorrowRecordStatus.Rejected &&
                borrowRecord.Status != BorrowRecordStatus.Returned
            );

            if (bookAlreadyBorrowed)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow request creation aborted. The user has already borrowed another copy of this book.");
                }

                return ApplicationErrors.AnotherCopyAlreadyBorrowed;
            }

            var copyAllocationResult = book.AllocateAvailableCopy();

            if (copyAllocationResult.IsError)
            {
                return copyAllocationResult.Errors!;
            }

            var copy = copyAllocationResult.Value;
            var borrowRecordCreationResult = BorrowRecord.Create(
                Guid.NewGuid(),
                request.UserId,
                copy.Id,
                request.DueDate,
                request.PickupDeadline,
                book.BorrowPricePerDay * (request.DueDate.DayNumber - today.DayNumber)
            );

            if (borrowRecordCreationResult.IsError)
            {
                return borrowRecordCreationResult.Errors!;
            }

            db.BorrowRecords.Add(borrowRecordCreationResult.Value);
            db.SetOriginalVersion(copy, copy.Version);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync(["book-copy"], cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Borrow request {BorrowRecordId} submitted.", borrowRecordCreationResult.Value.Id);
            }

            return borrowRecordCreationResult.Value.Id;
        }
    }
}
