using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateBookCopyStatus
{
    public sealed record UpdateBookCopyStatusCommand (
        Guid BookId,
        Guid CopyId,
        BookCopyStatus Status
    ) : IRequest<Result<Updated>>;
}
