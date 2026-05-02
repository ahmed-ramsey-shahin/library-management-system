using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.ChangeUserDetails
{
    public sealed record ChangeUserDetailsCommand(
        Guid UserId,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Address
    ) : IRequest<Result<Updated>>;
}
