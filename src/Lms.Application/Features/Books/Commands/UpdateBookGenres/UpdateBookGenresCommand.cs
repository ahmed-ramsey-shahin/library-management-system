using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateBookGenres
{
    public sealed record UpdateBookGenresCommand(Guid BookId, List<Guid> GenreIds) : IRequest<Result<Updated>>;
}
