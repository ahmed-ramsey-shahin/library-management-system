using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Queries.GenerateTokens
{
    public sealed record GenerateTokensQuery(string Email, string Password) : IRequest<Result<TokenDto>>;
}
