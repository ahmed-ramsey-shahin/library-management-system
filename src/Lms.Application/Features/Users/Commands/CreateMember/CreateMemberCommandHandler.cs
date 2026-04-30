using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Commands.CreateMember
{
    public sealed class CreateMemberCommandHandler(
        IAppDbContext db,
        ILogger<CreateMemberCommandHandler> logger,
        HybridCache cache,
        IPasswordHasher hasher
    ) : IRequestHandler<CreateMemberCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            var duplicateUser = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email == request.Email || user.PhoneNumber == request.PhoneNumber, cancellationToken);

            if (duplicateUser is not null)
            {
                if (duplicateUser.Email == request.Email)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("User creation aborted. Email {Email} already used by another user.", request.Email);
                    }

                    return ApplicationErrors.EmailIsUsed;
                }

                if (duplicateUser.PhoneNumber == request.PhoneNumber)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("User creation aborted. Phone number {PhoneNumber} already used by another user.", request.PhoneNumber);
                    }

                    return ApplicationErrors.PhoneNumberIsUsed;
                }
            }

            var userCreationResult = User.Create(
                Guid.NewGuid(),
                request.Email,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Address,
                User.GenerateLibraryNumber(),
                hasher.Hash(request.Password),
                Role.Member
            );

            if (userCreationResult.IsError)
            {
                return userCreationResult.Errors!;
            }

            var user = userCreationResult.Value;
            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("user", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("User {UserId} was created.", user.Id);
            }

            return user.Id;
        }
    }
}
