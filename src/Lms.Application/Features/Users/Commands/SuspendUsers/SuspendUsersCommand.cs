using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.SuspendUsers
{
    public sealed record SuspendUsersCommand : IRequest<Result<Updated>>;
}
