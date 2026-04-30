using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.ActivateUser
{
    public sealed record ActivateUserCommand(Guid UserId) : IRequest<Result<Updated>>;
}
