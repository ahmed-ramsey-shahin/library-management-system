using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Queries.RefreshToken
{
    public sealed record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) : IRequest<Result<TokenDto>>;
}
