using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.MarkBookCopyAsBorrowed
{
    public sealed record MarkBookCopyAsBorrowedCommand(
        Guid BookId,
        Guid CopyId
    ) : IRequest<Result<Updated>>;
}
