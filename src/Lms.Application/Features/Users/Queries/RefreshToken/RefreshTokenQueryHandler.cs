using System.Security.Claims;
using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Queries.RefreshToken
{
    public sealed class RefreshTokenQueryHandler(
        ILogger<RefreshTokenQueryHandler> logger,
        IIdentityService identityService,
        IAppDbContext db,
        ITokenProvider tokenProvider
    ) : IRequestHandler<RefreshTokenQuery, Result<TokenDto>>
    {
        public async Task<Result<TokenDto>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var principal = tokenProvider.GetPrincipalFromExpiredToken(request.ExpiredAccessToken);

            if (principal is null)
            {
                logger.LogWarning("Expired access token is not valid.");
                return ApplicationErrors.ExpiredAccessTokenInvalid;
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                logger.LogWarning("Invalid user id claim.");
                return ApplicationErrors.UserIdClaimInvalid;
            }

            if (!Guid.TryParse(userId, out Guid guidUserId))
            {
                return ApplicationErrors.UserIdInvalid;
            }

            var getUserResult = await identityService.GetUserByIdAsync(guidUserId, cancellationToken);

            if (getUserResult.IsError)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError("Could not get user {UserId}. {@Errors}.", guidUserId, getUserResult.Errors);
                }

                return getUserResult.Errors!;
            }

            var refreshToken = await db.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(token => token.Token == request.RefreshToken && token.UserId == guidUserId, cancellationToken);

            if (refreshToken is null || refreshToken.ExpiresOn < DateTimeOffset.UtcNow || refreshToken.IsRevoked)
            {
                logger.LogWarning("Refresh token has expired.");
                return ApplicationErrors.RefreshTokenExpired;
            }

            var generateTokenResult = await tokenProvider.GenerateJwtTokenAsync(getUserResult.Value, cancellationToken);

            if (generateTokenResult.IsError)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError("Generate token error. {@Error}.", generateTokenResult.Errors!);
                }

                return generateTokenResult.Errors!;
            }

            return generateTokenResult;
        }
    }
}
