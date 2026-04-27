using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Queries.GetBooksByGenre
{
    public sealed class GetBooksByGenreQueryHandler(
        IAppDbContext db,
        ILogger<GetBooksByGenreQueryHandler> logger
    ) : IRequestHandler<GetBooksByGenreQuery, Result<PaginatedList<BookSummaryDto>>>
    {
        public async Task<Result<PaginatedList<BookSummaryDto>>> Handle(GetBooksByGenreQuery request, CancellationToken cancellationToken)
        {
            var genre = await db.Genres.AsNoTracking().FirstOrDefaultAsync(genre => genre.Id == request.GenreId, cancellationToken);

            if (genre is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find genre with ID {GenreId}", request.GenreId);
                }

                return ApplicationErrors.GenreNotFound;
            }

            var booksQuery = db.Books
                .AsNoTracking()
                .Where(book => book.BookGenres.Any(bookGenre => bookGenre.GenreId == request.GenreId));
            var totalCount = await booksQuery.CountAsync(cancellationToken);
            var books = await booksQuery
                .OrderBy(book => book.Title)
                .ThenBy(book => book.Edition)
                .ThenBy(book => book.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(book => new BookSummaryDto
                {
                    BookId = book.Id,
                    Isbn = book.Isbn,
                    Title = book.Title,
                    Edition = book.Edition,
                    AvailableCopies = book.BookCopies.Count(copy => copy.State == BookCopyState.Available)
                }).ToListAsync(cancellationToken);
            return new PaginatedList<BookSummaryDto>
            {
                PageSize = request.PageSize,
                PageNumber = request.Page,
                TotalCount = totalCount,
                TotalPages = (int) Math.Ceiling((double) totalCount / request.PageSize),
                Items = books,
            };
        }
    }
}
