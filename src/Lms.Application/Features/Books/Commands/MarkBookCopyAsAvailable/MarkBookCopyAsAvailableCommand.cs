using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.MarkBookCopyAsAvailable
{
    public sealed record MarkBookCopyAsAvailableCommand(
        Guid BookId,
        Guid CopyId
    ) : IRequest<Result<Updated>>;
}
