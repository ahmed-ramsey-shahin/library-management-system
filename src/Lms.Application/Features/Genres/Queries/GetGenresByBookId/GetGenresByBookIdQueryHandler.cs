using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Genres.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Genres.Queries.GetGenresByBookId
{
    public sealed class GetGenresByBookIdQueryHandler(
        IAppDbContext db,
        ILogger<GetGenresByBookIdQueryHandler> logger
    ) : IRequestHandler<GetGenresByBookIdQuery, Result<List<GenreDto>>>
    {
        public async Task<Result<List<GenreDto>>> Handle(GetGenresByBookIdQuery request, CancellationToken cancellationToken)
        {
            var bookExists = await db.Books.AnyAsync(book => book.Id == request.BookId, cancellationToken);

            if (!bookExists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find book with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            return await db.Genres
                .Join(db.BookGenres, genres => genres.Id, bg => bg.GenreId, (genre, book) => new
                {
                    GenreId = genre.Id,
                    GenreName = genre.Name,
                    book.BookId,
                }).Where(genre => genre.BookId == request.BookId)
                .Select(genre => new GenreDto
                {
                    GenreId = genre.GenreId,
                    Name = genre.GenreName,
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
