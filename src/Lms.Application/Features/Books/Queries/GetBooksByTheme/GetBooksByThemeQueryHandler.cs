using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Queries.GetBooksByTheme
{
    public sealed class GetBooksByThemeQueryHandler(
        IAppDbContext db,
        ILogger<GetBooksByThemeQueryHandler> logger
    ) : IRequestHandler<GetBooksByThemeQuery, Result<PaginatedList<BookSummaryDto>>>
    {
        public async Task<Result<PaginatedList<BookSummaryDto>>> Handle(GetBooksByThemeQuery request, CancellationToken cancellationToken)
        {
            var theme = await db.Themes.AsNoTracking().FirstOrDefaultAsync(theme => theme.Id == request.ThemeId, cancellationToken);

            if (theme is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find theme with ID {ThemeId}", request.ThemeId);
                }

                return ApplicationErrors.ThemeNotFound;
            }

            var booksQuery = db.Books
                .AsNoTracking()
                .Where(book => book.BookThemes.Any(bookTheme => bookTheme.ThemeId == request.ThemeId));
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
