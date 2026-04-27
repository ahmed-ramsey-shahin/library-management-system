using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Commands.BorrowBook
{
    public sealed class BorrowBookCommandHandler(
        IAppDbContext db,
        ILogger<BorrowBookCommandHandler> logger,
        HybridCache cache,
        IConfiguration config
    ) : IRequestHandler<BorrowBookCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
            var member = await db.Users
                .AsNoTracking()
                .Include(user => user.BorrowRecords)
                .ThenInclude(borrowRecord => borrowRecord.BookCopy)
                .ThenInclude(copy => copy.Book)
                .Include(user => user.Fines)
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

            var activeBorrows = member.BorrowRecords.Count(borrowRecord => borrowRecord.Status == BorrowRecordStatus.Accepted);
            var lateBorrows = member.BorrowRecords.Count(borrowRecord => borrowRecord.Status == BorrowRecordStatus.Late);
            var unpaidFine = member.Fines.Count(fine => fine.Status == FineStatus.Unpaid);
            var maximumActiveBorrows = int.Parse(config["maximumActiveBorrows"]!);
            var maximumLateBorrows = int.Parse(config["maximumLateBorrows"]!);
            var maximumUnpaidFines = int.Parse(config["maximumUnpaidFines"]!);

            if (activeBorrows >= maximumActiveBorrows)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow request creation aborted. User {UserId} has {ActiveBorrows} currently active borrows.", request.UserId, activeBorrows);
                }

                return ApplicationErrors.ActiveBorrowsLimitReached;
            }

            if (lateBorrows >= maximumLateBorrows)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow request creation aborted. User {UserId} has {LateBorrows} currently late borrows.", request.UserId, lateBorrows);
                }

                return ApplicationErrors.LateBorrowsLimitReached;
            }

            if (unpaidFine >= maximumUnpaidFines)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow request creation aborted. User {UserId} has unpaid fines.", request.UserId);
                }

                return ApplicationErrors.UnpaidFinesLimitReached;
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
                book.BorrowPricePerDay * (request.DueDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber)
            );

            if (borrowRecordCreationResult.IsError)
            {
                return borrowRecordCreationResult.Errors!;
            }

            db.BorrowRecords.Add(borrowRecordCreationResult.Value);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync(["book-copy"], cancellationToken);
            return borrowRecordCreationResult.Value.Id;
        }
    }
}
