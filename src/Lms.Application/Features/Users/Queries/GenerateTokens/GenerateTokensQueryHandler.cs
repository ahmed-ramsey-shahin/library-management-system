using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Queries.GenerateTokens
{
    public sealed class GenerateTokensQueryHandler(
        ILogger<GenerateTokensQueryHandler> logger,
        ITokenProvider tokenProvider,
        IIdentityService identityService
    ) : IRequestHandler<GenerateTokensQuery, Result<TokenDto>>
    {
        public async Task<Result<TokenDto>> Handle(GenerateTokensQuery request, CancellationToken cancellationToken)
        {
            var userResponse = await identityService.AuthenticateAsync(request.Email, request.Password, cancellationToken);

            if (userResponse.IsError)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError("Could not authenticate user {UserEmail}. {@Errors}.", request.Email, userResponse.Errors!);
                }

                return userResponse.Errors!;
            }

            var generateTokenResult = await tokenProvider.GenerateJwtTokenAsync(userResponse.Value, cancellationToken);

            if (generateTokenResult.IsError)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError("Could not generate token for user {UserEmail}. {@Errors}.", request.Email, generateTokenResult.Errors!);
                }

                return generateTokenResult.Errors!;
            }

            return generateTokenResult;
        }
    }
}
