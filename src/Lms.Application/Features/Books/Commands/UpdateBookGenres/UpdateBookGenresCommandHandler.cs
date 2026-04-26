using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.UpdateBookGenres
{
    public sealed class UpdateBookGenresCommandHandler(
        IAppDbContext db,
        ILogger<UpdateBookGenresCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<UpdateBookGenresCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateBookGenresCommand request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .Include(book => book.BookGenres)
                .FirstOrDefaultAsync(book => book.Id == request.BookId, cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book update aborted. No book was found with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            var genreIds = request.GenreIds.Distinct().ToList();

            if (genreIds.Count > 0)
            {
                var existingGenres = await db.Genres.CountAsync(
                    a => genreIds.Contains(a.Id),
                    cancellationToken
                );

                if (existingGenres != genreIds.Count)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning("Book update aborted. One or more genres are invalid.");
                    }

                    return ApplicationErrors.GenreNotFound;
                }
            }

            var updateResult = book.UpsertGenres(genreIds);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Updated genres of book {BookId}.", request.BookId);
            }

            return Result.Updated;
        }
    }
}
