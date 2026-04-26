using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateBookCopyLocation
{
    public sealed record UpdateBookCopyLocationCommand(
        Guid BookId,
        Guid CopyId,
        string Location,
        byte[] Version
    ) : IRequest<Result<Updated>>;
}
