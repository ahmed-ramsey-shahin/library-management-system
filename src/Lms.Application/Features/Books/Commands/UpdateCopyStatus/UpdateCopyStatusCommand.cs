using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateCopyStatus
{
    public sealed record UpdateCopyStatusCommand(
        Guid BookId,
        Guid CopyId,
        BookCopyStatus Status
    ) : IRequest<Result<Updated>>;
}
