using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.ChangeUserPassword
{
    public sealed record ChangeUserPasswordCommand(Guid UserId, string OldPassword, string NewPassword) : IRequest<Result<Updated>>;
}
