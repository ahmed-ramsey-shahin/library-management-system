using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.AllocateAvailableCopy
{
    public sealed record AllocateAvailableCopyCommand(
        Guid BookId
    ) : IRequest<Result<Guid>>;
}
