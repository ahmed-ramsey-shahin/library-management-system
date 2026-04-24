using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Genres.Dtos;
using Lms.Application.Features.Genres.Mappers;
using Lms.Domain.Common.Results;
using Lms.Domain.Metadata;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Genres.Commands.CreateGenre
{
    public class CreateGenreCommandHandler(
        IAppDbContext db,
        ILogger<CreateGenreCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<CreateGenreCommand, Result<GenreDto>>
    {
        public async Task<Result<GenreDto>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
        {
            var exists = await db.Genres.AnyAsync(genre => string.Equals(genre.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Genre creation aborted. Genre already exists");
                }

                return ApplicationErrors.GenreAlreadyExists;
            }

            var genreCreationResult = Genre.Create(Guid.NewGuid(), request.Name);

            if (genreCreationResult.IsError)
            {
                return genreCreationResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("genre", cancellationToken);
            var genre = genreCreationResult.Value.ToDto();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Genre created successfully. Id: {GenreId}", genre.GenreId);
            }

            return genre;
        }
    }
}
