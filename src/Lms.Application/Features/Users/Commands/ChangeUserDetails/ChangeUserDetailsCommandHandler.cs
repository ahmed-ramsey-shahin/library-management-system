using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Commands.ChangeUserDetails
{
    public sealed class ChangeUserDetailsCommandHandler(
        IAppDbContext db,
        ILogger<ChangeUserDetailsCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<ChangeUserDetailsCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(ChangeUserDetailsCommand request, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("User update aborted. User {UserId} was not found.", request.UserId);
                }

                return ApplicationErrors.UserNotFound;
            }

            if (user.PhoneNumber != request.PhoneNumber)
            {
                var phoneNumberExists = await db.Users
                    .AnyAsync(user => user.PhoneNumber == request.PhoneNumber && user.Id != request.UserId, cancellationToken);

                if (phoneNumberExists)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("User update aborted. Phone number {PhoneNumber} already used by another user.", request.PhoneNumber);
                    }

                    return ApplicationErrors.PhoneNumberIsUsed;
                }
            }

            var updateResult = user.ChangePersonalDetails(
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Address
            );

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("user", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("User {UserId} updated.", request.UserId);
            }

            return Result.Updated;
        }
    }
}
