using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Queries.GetBookCopyById
{
    public class GetBookCopyByIdQueryHandler(
        ILogger<GetBookCopyByIdQueryHandler> logger,
        IAppDbContext db
    ) : IRequestHandler<GetBookCopyByIdQuery, Result<BookCopyDto>>
    {
        public async Task<Result<BookCopyDto>> Handle(GetBookCopyByIdQuery request, CancellationToken cancellationToken)
        {
            var bookCopy = await db.BookCopies
                .AsNoTracking()
                .Where(copy => copy.Id == request.BookCopyId)
                .Select(copy => new BookCopyDto
                {
                    Status = copy.Status,
                    Barcode = copy.Barcode,
                    Location = copy.Location,
                    State = copy.State,
                    AcquisitionDate = copy.AcquisitionDate,
                    Version = copy.Version,
                    Book = new BookSummaryDto
                    {
                        Edition = copy.Book.Edition,
                        Isbn = copy.Book.Isbn,
                        Title = copy.Book.Title,
                        AvailableCopies = db.BookCopies.Count(c => c.BookId == copy.BookId && c.State == BookCopyState.Available),
                        BookId = copy.BookId,
                    }
                }).FirstOrDefaultAsync(cancellationToken);

            if (bookCopy is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("No book copy was found with id {BookCopyId}.", request.BookCopyId);
                }

                return ApplicationErrors.BookCopyNotFound;
            }

            return bookCopy;
        }
    }
}
