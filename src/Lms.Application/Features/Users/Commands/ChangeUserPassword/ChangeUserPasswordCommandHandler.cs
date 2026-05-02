using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Commands.ChangeUserPassword
{
    public sealed class ChangeUserPasswordCommandHandler(
        IAppDbContext db,
        ILogger<ChangeUserPasswordCommandHandler> logger,
        IPasswordHasher hasher
    ) : IRequestHandler<ChangeUserPasswordCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
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

            if (!hasher.Verify(request.OldPassword, user.Password))
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("User update aborted. Passwords do not match.");
                }

                return ApplicationErrors.PasswordsDontMatch;
            }

            var newPassword = hasher.Hash(request.NewPassword);
            var updateResult = user.ChangePassword(newPassword);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("User {UserId} password was updated.", request.UserId);
            }

            return Result.Updated;
        }
    }
}
