using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.CreateMember
{
    public sealed record CreateMemberCommand(
        string Email,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Address,
        string Password,
        Guid IdempotencyKey
    ) : IRequest<Result<Guid>>, IIdempotentCommand;
}
