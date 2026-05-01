using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Users.Commands.UpdateLibrarianCategories
{
    public sealed record UpdateLibrarianCategoriesCommand(
        Guid LibrarianId,
        List<Guid> CategoryIds
    ) : IRequest<Result<Updated>>;
}
