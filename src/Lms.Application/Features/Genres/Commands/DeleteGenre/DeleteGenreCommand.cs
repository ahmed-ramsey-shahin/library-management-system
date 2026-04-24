using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Genres.Commands.DeleteGenre
{
    public sealed record DeleteGenreCommand(Guid GenreId) : IRequest<Result<Deleted>>;
}
