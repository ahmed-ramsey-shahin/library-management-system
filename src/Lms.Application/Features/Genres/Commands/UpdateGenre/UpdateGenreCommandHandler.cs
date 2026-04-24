using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Genres.Commands.UpdateGenre
{
    public sealed class UpdateGenreCommandHandler(
        ILogger<UpdateGenreCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<UpdateGenreCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
        {
            var genre = await db.Genres.FirstOrDefaultAsync(genre => genre.Id == request.GenreId, cancellationToken);

            if (genre is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find genre with ID {GenreId}", request.GenreId);
                }

                return ApplicationErrors.GenreNotFound;
            }

            var exists = await db.Genres.AnyAsync(genre => string.Equals(genre.Name, request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (exists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Genre creation aborted. Genre already exists");
                }

                return ApplicationErrors.GenreAlreadyExists;
            }

            genre.Update(request.Name);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("genre", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Genre {GenreId} updated successfully.", genre.Id);
            }

            return Result.Updated;
        }
    }
}
