using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Queries.GetBooksByKeyword
{
    public sealed class GetBooksByKeywordQueryHandler(
        IAppDbContext db,
        ILogger<GetBooksByKeywordQueryHandler> logger
    ) : IRequestHandler<GetBooksByKeywordQuery, Result<PaginatedList<BookSummaryDto>>>
    {
        public async Task<Result<PaginatedList<BookSummaryDto>>> Handle(GetBooksByKeywordQuery request, CancellationToken cancellationToken)
        {
            var keyword = await db.Keywords.AsNoTracking().FirstOrDefaultAsync(keyword => keyword.Id == request.KeywordId, cancellationToken);

            if (keyword is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find keyword with ID {KeywordId}", request.KeywordId);
                }

                return ApplicationErrors.KeywordNotFound;
            }

            var booksQuery = db.Books
                .AsNoTracking()
                .Where(book => book.BookKeywords.Any(bookKeyword => bookKeyword.KeywordId == request.KeywordId));
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
