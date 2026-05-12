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
        IPasswordHasher hasher,
        IPasswordGenerator passwordGenerator,
        IEmailService emailService
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

            var password = passwordGenerator.Generate();
            var passwordHash = hasher.Hash(password);
            var updateResult = user.ChangePassword(passwordHash);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await db.RefreshTokens.Where(token => token.UserId == request.UserId)
                .ExecuteDeleteAsync(cancellationToken);
            _ = emailService.SendEmailAsync(user.Email, user.FirstName, "Your password has been reseted by an admin.", $"Your new password is {password}. Please change it ASAP.", default);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("User {UserId} password was reset.", request.UserId);
            }

            return Result.Updated;
        }
    }
}
