using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Errors;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.UpdateBookDetails
{
    public sealed class UpdateBookDetailsCommandHandler(
        IAppDbContext db,
        HybridCache cache,
        ILogger<UpdateBookDetailsCommandHandler> logger
    ) : IRequestHandler<UpdateBookDetailsCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(
            UpdateBookDetailsCommand request,
            CancellationToken cancellationToken
        )
        {
            var book = await db.Books.FirstOrDefaultAsync(
                book => book.Id == request.BookId,
                cancellationToken
            );

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning(
                        "Book update aborted. No book was found with ID {BookId}",
                        request.BookId
                    );
                }

                return ApplicationErrors.BookNotFound;
            }

            var isbn = request.Isbn ?? book.Isbn;
            var issn = request.Issn ?? book.Issn;
            var title = request.Title ?? book.Title;
            var description = request.Description ?? book.Description;
            var pageCount = request.PageCount ?? book.PageCount;
            var publisherId = request.PublisherId ?? book.PublisherId;
            var publishingDate = request.PublishingDate ?? book.PublishingDate;
            var edition = request.Edition ?? book.Edition;
            var language = request.Language ?? book.Language;

            if (request.PublisherId is not null && request.PublisherId != book.PublisherId)
            {
                var publisherExists = await db.Publishers.AnyAsync(
                    p => p.Id == publisherId,
                    cancellationToken
                );

                if (!publisherExists)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning(
                            "Book update aborted. Publisher {PublisherId} does not exist.",
                            request.PublisherId
                        );
                    }

                    return ApplicationErrors.PublisherNotFound;
                }
            }

            if ((request.Isbn is not null && request.Isbn != book.Isbn) || (request.Issn is not null && request.Issn != book.Issn))
            {
                var duplicateBook = await db.Books
                    .Where(book => book.Id != request.BookId && (book.Isbn == isbn || book.Issn == issn))
                    .Select(book => new { book.Isbn, book.Issn })
                    .FirstOrDefaultAsync(cancellationToken);

                if (duplicateBook is not null)
                {
                    if (duplicateBook.Isbn == isbn)
                    {
                        if (logger.IsEnabled(LogLevel.Warning))
                        {
                            logger.LogWarning(
                                "Book update aborted. A book with {ISBN} already exists.",
                                isbn
                            );
                        }

                        return ApplicationErrors.IsbnAlreadyExists;
                    }

                    if (duplicateBook.Issn == issn)
                    {
                        if (logger.IsEnabled(LogLevel.Warning))
                        {
                            logger.LogWarning(
                                "Book update aborted. A book with {ISSN} already exists.",
                                issn
                            );
                        }

                        return ApplicationErrors.IssnAlreadyExists;
                    }
                }
            }

            var result = book.UpdateDetails(
                isbn: isbn,
                issn: issn,
                title: title,
                description: description,
                pageCount: pageCount,
                publisherId: publisherId,
                publishingDate: publishingDate,
                edition: edition,
                language: language
            );

            if (result.IsError)
            {
                return result.Errors!;
            }

            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Book {BookId} details was updated.", request.BookId);
            }

            return Result.Updated;
        }
    }
}
