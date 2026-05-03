using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Queries.GetUserByEmail
{
    public sealed class GetUserByEmailQueryHandler(
        IAppDbContext db,
        ILogger<GetUserByEmailQueryHandler> logger
    ) : IRequestHandler<GetUserByEmailQuery, Result<UserDto>>
    {
        public async Task<Result<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .AsNoTracking()
                .Where(user => user.Email == request.Email)
                .Select(user => new UserDto
                {
                    Email = user.Email,
                    UserId = user.Id,
                    Status = user.Status,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    LibraryCardNumber = user.LibraryCardNumber,
                    PasswordHash = user.Password,
                    Role = user.Role
                }).FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("No user was found with email {Email}.", request.Email);
                }

                return ApplicationErrors.UserNotFound;
            }

            return user;
        }
    }
}
