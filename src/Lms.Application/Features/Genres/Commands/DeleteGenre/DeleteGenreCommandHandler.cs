using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Genres.Commands.DeleteGenre
{
    public sealed class DeleteGenreCommandHandler(
        ILogger<DeleteGenreCommandHandler> logger,
        IAppDbContext db,
        HybridCache cache
    ) : IRequestHandler<DeleteGenreCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
        {
            var genre = await db.Genres.FirstOrDefaultAsync(genre => request.GenreId == genre.Id, cancellationToken);

            if (genre is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find genre with ID {GenreId}", request.GenreId);
                }

                return ApplicationErrors.GenreNotFound;
            }

            var associatedBooks = await db.BookGenres.CountAsync(bg => bg.GenreId == request.GenreId, cancellationToken);
            var deletionResult = genre.Delete(associatedBooks);

            if (deletionResult.IsError)
            {
                return deletionResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("genre", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Deleted genre {GenreId}", request.GenreId);
            }

            return Result.Deleted;
        }
    }
}
