using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Commands.AdminResetUserPassword
{
    public sealed class AdminResetUserPasswordCommandHandler(
        IAppDbContext db,
        ILogger<AdminResetUserPasswordCommandHandler> logger,
        IPasswordHasher hasher
    ) : IRequestHandler<AdminResetUserPasswordCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(AdminResetUserPasswordCommand request, CancellationToken cancellationToken)
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

            var passwordHash = hasher.Hash(request.Password);
            var updateResult = user.ChangePassword(passwordHash);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("User {UserId} password was reset.", request.UserId);
            }

            return Result.Updated;
        }
    }
}
