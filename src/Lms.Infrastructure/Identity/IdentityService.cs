using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lms.Infrastructure.Identity
{
    public class IdentityService(IAppDbContext db, IPasswordHasher passwordHasher) : IIdentityService
    {
        public async Task<Result<UserDto>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .AsNoTracking()
                .Where(user => user.Email == email)
                .Select(user => new UserDto
                {
                    Email = user.Email,
                    Status = user.Status,
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    LibraryCardNumber = user.LibraryCardNumber,
                    PasswordHash = user.Password,
                    Role = user.Role
                }).FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return ApplicationErrors.UserNotFound;
            }

            var isPasswordValid = passwordHasher.Verify(password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return ApplicationErrors.CredentialsInvalid;
            }

            if (user.Status == UserStatus.Suspended)
            {
                return UserErrors.UserSuspended;
            }

            return user;
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .AsNoTracking()
                .Where(user => user.Id == userId)
                .Select(user => new UserDto
                {
                    Email = user.Email,
                    Status = user.Status,
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    LibraryCardNumber = user.LibraryCardNumber,
                    PasswordHash = user.Password,
                    Role = user.Role
                }).FirstOrDefaultAsync(cancellationToken);

            return user ?? (Result<UserDto>)ApplicationErrors.UserNotFound;
        }
    }
}
