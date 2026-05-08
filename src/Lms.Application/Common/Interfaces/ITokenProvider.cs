using System.Security.Claims;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Common.Interfaces
{
    public interface ITokenProvider
    {
        Task<Result<TokenDto>> GenerateJwtTokenAsync(UserDto user, CancellationToken cancellationToken);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
