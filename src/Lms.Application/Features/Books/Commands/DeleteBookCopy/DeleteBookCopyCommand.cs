using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.DeleteBookCopy
{
    public sealed record DeleteBookCopyCommand(Guid BookId, Guid CopyId) : IRequest<Result<Deleted>>;
}
