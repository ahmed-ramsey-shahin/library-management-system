using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<UserDto>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken);
        Task<Result<UserDto>> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
    }
}
