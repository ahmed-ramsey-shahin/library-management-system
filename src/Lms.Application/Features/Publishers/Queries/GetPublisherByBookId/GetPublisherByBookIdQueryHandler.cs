using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Publishers.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Publishers.Queries.GetPublisherByBookId
{
    public sealed class GetPublisherByBookIdQueryHandler(
        IAppDbContext db,
        ILogger<GetPublisherByBookIdQueryHandler> logger
    ) : IRequestHandler<GetPublisherByBookIdQuery, Result<List<PublisherDto>>>
    {
        public async Task<Result<List<PublisherDto>>> Handle(GetPublisherByBookIdQuery request, CancellationToken cancellationToken)
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

            return await db.Publishers
                .Join(db.Books, publishers => publishers.Id, book => book.PublisherId, (publisher, book) => new
                {
                    PublisherId = publisher.Id,
                    PublisherName = publisher.Name,
                    BookId = book.Id,
                }).Where(publisher => publisher.BookId == request.BookId)
                .Select(publisher => new PublisherDto
                {
                    PublisherId = publisher.PublisherId,
                    Name = publisher.PublisherName,
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
