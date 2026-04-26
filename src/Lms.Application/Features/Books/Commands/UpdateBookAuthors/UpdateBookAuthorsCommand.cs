using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateBookAuthors
{
    public sealed record UpdateBookAuthorsCommand(Guid BookId, List<Guid> AuthorIds) : IRequest<Result<Updated>>;
}
