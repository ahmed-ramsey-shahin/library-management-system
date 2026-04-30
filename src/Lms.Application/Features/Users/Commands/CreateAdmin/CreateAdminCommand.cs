using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.CreateAdmin
{
    public sealed record CreateAdminCommand(
        string Email,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Address,
        string Password
    ) : IRequest<Result<Guid>>;
}
