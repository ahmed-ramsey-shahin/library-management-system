using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.CreateLibrarian
{
    public sealed record CreateLibrarianCommand(
        string Email,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Address,
        string Password,
        List<Guid> CategoryIds
    ) : IRequest<Result<Guid>>;
}
