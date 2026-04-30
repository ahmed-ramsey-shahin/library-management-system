using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Commands.SuspendUser
{
    public sealed class SuspendUserCommandHandler(
        IAppDbContext db,
        ILogger<SuspendUserCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<SuspendUserCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(SuspendUserCommand request, CancellationToken cancellationToken)
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

            var updateResult = user.Suspend();

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("user", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("User {UserId} was suspended.", request.UserId);
            }

            return Result.Updated;
        }
    }
}
