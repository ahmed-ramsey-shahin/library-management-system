using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.DeleteUser
{
    public sealed record DeleteUserCommand (Guid UserId) : IRequest<Result<Deleted>>;
}
