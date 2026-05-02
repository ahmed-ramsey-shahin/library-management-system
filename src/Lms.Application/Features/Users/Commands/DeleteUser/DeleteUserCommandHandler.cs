using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Commands.DeleteUser
{
    public sealed class DeleteUserCommandHandler(
        IAppDbContext db,
        ILogger<DeleteUserCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<DeleteUserCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("User deletion aborted. User {UserId} was not found.", request.UserId);
                }

                return ApplicationErrors.UserNotFound;
            }

            var hasActiveBorrows = false;
            var hasUnpaidFines = false;

            if (user.Role == Role.Member)
            {
                hasUnpaidFines = await db.Fines
                    .AnyAsync(fine => fine.MemberId == request.UserId && fine.Status == FineStatus.Unpaid, cancellationToken);

                hasActiveBorrows = await db.BorrowRecords
                    .AnyAsync(record => record.MemberId == request.UserId && (
                        record.Status == BorrowRecordStatus.Late ||
                        record.Status == BorrowRecordStatus.Accepted ||
                        record.Status == BorrowRecordStatus.Waiting
                    ), cancellationToken);
            }

            var deletionResult = user.Delete(hasActiveBorrows, hasUnpaidFines);

            if (deletionResult.IsError)
            {
                return deletionResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("user", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("User {UserId} was deleted.", request.UserId);
            }

            return Result.Deleted;
        }
    }
}
