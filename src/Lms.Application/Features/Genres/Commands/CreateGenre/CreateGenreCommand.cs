using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Genres.Dtos;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Genres.Commands.CreateGenre
{
    public sealed record CreateGenreCommand(string Name, Guid IdempotencyKey) : IRequest<Result<GenreDto>>, IIdempotentCommand;
}
