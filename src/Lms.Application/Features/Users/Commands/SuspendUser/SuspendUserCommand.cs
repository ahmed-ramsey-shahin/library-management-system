using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.SuspendUser
{
    public sealed record SuspendUserCommand(Guid UserId) : IRequest<Result<Updated>>;
}
