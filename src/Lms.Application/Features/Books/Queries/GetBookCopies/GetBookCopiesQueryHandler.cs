using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Queries.GetBookCopies
{
    public class GetBookCopiesQueryHandler(
        IAppDbContext db,
        ILogger<GetBookCopiesQueryHandler> logger
    ) : IRequestHandler<GetBookCopiesQuery, Result<List<BookCopySummaryDto>>>
    {
        public async Task<Result<List<BookCopySummaryDto>>> Handle(GetBookCopiesQuery request, CancellationToken cancellationToken)
        {
            var bookExists = await db.Books.AnyAsync(book => book.Id == request.BookId, cancellationToken);

            if (!bookExists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("No book was found with id {BookId}.", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            return await db.BookCopies
                .Where(copy => copy.BookId == request.BookId)
                .AsNoTracking()
                .Select(copy => new BookCopySummaryDto
                {
                    Status = copy.Status,
                    AcquisitionDate = copy.AcquisitionDate,
                    Barcode = copy.Barcode,
                    Location = copy.Location,
                    State = copy.State,
                    Version = copy.Version
                }).ToListAsync(cancellationToken);
        }

    }
}
