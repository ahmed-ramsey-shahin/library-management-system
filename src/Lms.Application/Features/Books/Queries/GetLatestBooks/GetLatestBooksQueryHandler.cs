using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Books.Queries.GetLatestBooks
{
    public sealed class GetLatestBooksQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetLatestBooksQuery, Result<PaginatedList<BookSummaryDto>>>
    {
        public async Task<Result<PaginatedList<BookSummaryDto>>> Handle(GetLatestBooksQuery request, CancellationToken cancellationToken)
        {
            var booksQuery = db.Books
                .AsNoTracking()
                .OrderByDescending(book => book.CreatedAt)
                .Take(50);
            var totalCount = await booksQuery.CountAsync(cancellationToken);
            var books = await booksQuery
                .OrderByDescending(book => book.CreatedAt)
                .ThenBy(book => book.Title)
                .ThenBy(book => book.Language)
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
                Items = books,
                TotalPages = (int) Math.Ceiling((double)totalCount / request.PageSize)
            };
        }
    }
}
