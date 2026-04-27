using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Queries.GetBooksByPublisher
{
    public sealed class GetBooksByPublisherQueryHandler(
        IAppDbContext db,
        ILogger<GetBooksByPublisherQueryHandler> logger
    ) : IRequestHandler<GetBooksByPublisherQuery, Result<PaginatedList<BookSummaryDto>>>
    {
        public async Task<Result<PaginatedList<BookSummaryDto>>> Handle(GetBooksByPublisherQuery request, CancellationToken cancellationToken)
        {
            var publisher = await db.Publishers.AsNoTracking().FirstOrDefaultAsync(publisher => publisher.Id == request.PublisherId, cancellationToken);

            if (publisher is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find publisher with ID {PublisherId}", request.PublisherId);
                }

                return ApplicationErrors.PublisherNotFound;
            }

            var booksQuery = db.Books
                .AsNoTracking()
                .Where(book => book.PublisherId == request.PublisherId);
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
