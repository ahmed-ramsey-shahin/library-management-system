using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Genres.Commands.UpdateGenre
{
    public sealed record UpdateGenreCommand(Guid GenreId, string Name) : IRequest<Result<Updated>>;
}
