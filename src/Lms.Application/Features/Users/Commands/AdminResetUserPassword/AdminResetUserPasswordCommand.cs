using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.AdminResetUserPassword
{
    public sealed record AdminResetUserPasswordCommand(
        Guid UserId,
        string Password
    ) : IRequest<Result<Updated>>;
}
