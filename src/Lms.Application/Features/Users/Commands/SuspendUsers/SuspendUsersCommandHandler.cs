using Lms.Application.Common.Interfaces;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Commands.SuspendUsers
{
    public sealed class SuspendUsersCommandHandler(
        IAppDbContext db,
        ILogger<SuspendUsersCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<SuspendUsersCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(SuspendUsersCommand request, CancellationToken cancellationToken)
        {
            var usersToSuspend = await db.Users
                .Where(user => user.Role == Role.Member && user.Status != UserStatus.Suspended && user.Fines.Any(
                    fine => fine.Status == FineStatus.Unpaid &&
                            fine.FineDate < DateTimeOffset.UtcNow.AddDays(-30)
                )).ToListAsync(cancellationToken);
            var suspendedUsers = 0;

            foreach (var user in usersToSuspend)
            {
                var suspensionResult = user.Suspend();

                if (suspensionResult.IsError && logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError("User {UserId} can not be suspended. {@Errors}.", user.Id, suspensionResult.Errors);
                }
                else
                {
                    suspendedUsers++;
                }
            }

            if (suspendedUsers > 0)
            {
                await db.SaveChangesAsync(cancellationToken);
                await cache.RemoveByTagAsync("user", cancellationToken);
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("{NumberOfSuspendedUsers} users were suspended.", suspendedUsers);
            }

            return Result.Updated;
        }
    }
}
