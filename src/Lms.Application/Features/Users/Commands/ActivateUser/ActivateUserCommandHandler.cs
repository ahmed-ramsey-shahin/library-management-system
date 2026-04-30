using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Commands.ActivateUser
{
    public sealed class ActivateUserCommandHandler(
        IAppDbContext db,
        ILogger<ActivateUserCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<ActivateUserCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await db.Users.FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("User update aborted. User {UserId} was not found.", request.UserId);
                }

                return ApplicationErrors.UserNotFound;
            }

            if (user.Role == Role.Member)
            {
                var unpaidFines = await db.Fines.AnyAsync(fine => fine.MemberId == request.UserId && fine.Status == FineStatus.Unpaid, cancellationToken);

                if (unpaidFines)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Cannot activate user {UserId}. The user still has unpaid fines.", request.UserId);
                    }

                    return ApplicationErrors.UserHasUnpaidFines;
                }
            }

            var activationResult = user.Activate();

            if (activationResult.IsError)
            {
                return activationResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("user", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("User {UserId} activated.", request.UserId);
            }

            return Result.Updated;
        }
    }
}
