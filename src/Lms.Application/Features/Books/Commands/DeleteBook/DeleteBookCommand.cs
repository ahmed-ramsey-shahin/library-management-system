using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.DeleteBook
{
    public sealed record DeleteBookCommand(Guid BookId) : IRequest<Result<Deleted>>;
}
