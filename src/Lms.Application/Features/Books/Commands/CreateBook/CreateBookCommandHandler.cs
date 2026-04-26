using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Books.Commands.CreateBook
{
    public sealed class CreateBookCommandHandler(
        IAppDbContext db,
        ILogger<CreateBookCommandHandler> logger,
        HybridCache cache
    ) : IRequestHandler<CreateBookCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(
            CreateBookCommand request,
            CancellationToken cancellationToken
        )
        {
            var publisherExists = await db.Publishers.AnyAsync(
                p => p.Id == request.PublisherId,
                cancellationToken
            );

            if (!publisherExists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning(
                        "Book creation aborted. Publisher {PublisherId} does not exist.",
                        request.PublisherId
                    );
                }

                return ApplicationErrors.PublisherNotFound;
            }

            var authorIds = request.AuthorIds.Distinct().ToList();
            var existingAuthors = await db.Authors.CountAsync(
                a => authorIds.Contains(a.Id),
                cancellationToken
            );

            if (existingAuthors != authorIds.Count)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book creation aborted. One or more authors are invalid.");
                }

                return ApplicationErrors.AuthorNotFound;
            }

            var categoryIds = request.CategoryIds.Distinct().ToList();
            var existingCategories = await db.Categories.CountAsync(
                c => categoryIds.Contains(c.Id),
                cancellationToken
            );

            if (existingCategories != categoryIds.Count)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book creation aborted. One or more categories are invalid.");
                }

                return ApplicationErrors.CategoryNotFound;
            }

            var keywordIds = request.KeywordIds.Distinct().ToList();
            var existingKeywords = await db.Keywords.CountAsync(
                k => keywordIds.Contains(k.Id),
                cancellationToken
            );

            if (existingKeywords != keywordIds.Count)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book creation aborted. One or more keywords are invalid.");
                }

                return ApplicationErrors.KeywordNotFound;
            }

            var themeIds = request.ThemeIds.Distinct().ToList();
            var existingThemes = await db.Themes.CountAsync(
                t => themeIds.Contains(t.Id),
                cancellationToken
            );

            if (existingThemes != themeIds.Count)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book creation aborted. One or more themes are invalid.");
                }

                return ApplicationErrors.ThemeNotFound;
            }

            var genreIds = request.GenreIds.Distinct().ToList();
            var existingGenres = await db.Genres.CountAsync(
                g => genreIds.Contains(g.Id),
                cancellationToken
            );

            if (existingGenres != genreIds.Count)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book creation aborted. One more genres are invalid.");
                }

                return ApplicationErrors.GenreNotFound;
            }

            var audienceIds = request.AudienceIds.Distinct().ToList();
            var existingAudiences = await db.Audiences.CountAsync(
                a => audienceIds.Contains(a.Id),
                cancellationToken
            );

            if (existingAudiences != audienceIds.Count)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book creation aborted. One or more audiences are invalid.");
                }

                return ApplicationErrors.AudienceNotFound;
            }

            var duplicateBook = await db
                .Books.Where(book => book.Isbn == request.Isbn || book.Issn == request.Issn)
                .Select(book => new { book.Isbn, book.Issn })
                .FirstOrDefaultAsync(cancellationToken);

            if (duplicateBook is not null)
            {
                if (duplicateBook.Isbn == request.Isbn)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning(
                            "Book creation aborted. A book with {ISBN} already exists.",
                            request.Isbn
                        );
                    }

                    return ApplicationErrors.IsbnAlreadyExists;
                }

                if (duplicateBook.Issn == request.Issn)
                {
                    if (logger.IsEnabled(LogLevel.Warning))
                    {
                        logger.LogWarning(
                            "Book creation aborted. A book with {ISSN} already exists.",
                            request.Issn
                        );
                    }

                    return ApplicationErrors.IssnAlreadyExists;
                }
            }

            var bookCreationResult = Book.Create(
                id: Guid.NewGuid(),
                isbn: request.Isbn,
                issn: request.Issn,
                title: request.Title,
                description: request.Description,
                pageCount: request.PageCount,
                publisherId: request.PublisherId,
                publishingDate: request.PublishingDate,
                edition: request.Edition,
                borrowPricePerDay: request.BorrowPricePerDay,
                finePerDay: request.FinePerDay,
                lostFee: request.LostFee,
                damageFee: request.DamageFee,
                language: request.Language
            );

            if (bookCreationResult.IsError)
            {
                return bookCreationResult.Errors!;
            }

            var book = bookCreationResult.Value;

            var upsertingAudiencesResult = book.UpsertAudiences(audienceIds);

            if (upsertingAudiencesResult.IsError)
            {
                return upsertingAudiencesResult.Errors!;
            }

            var upsertingAuthorsResult = book.UpsertAuthors(authorIds);

            if (upsertingAuthorsResult.IsError)
            {
                return upsertingAuthorsResult.Errors!;
            }

            var upsertingCategoriesResult = book.UpsertCategories(categoryIds);

            if (upsertingCategoriesResult.IsError)
            {
                return upsertingCategoriesResult.Errors!;
            }

            var upsertingGenresResult = book.UpsertGenres(genreIds);

            if (upsertingGenresResult.IsError)
            {
                return upsertingGenresResult.Errors!;
            }

            var upsertingKeywordsResult = book.UpsertKeywords(keywordIds);

            if (upsertingKeywordsResult.IsError)
            {
                return upsertingKeywordsResult.Errors!;
            }

            var upsertingThemesResult = book.UpsertThemes(themeIds);

            if (upsertingThemesResult.IsError)
            {
                return upsertingThemesResult.Errors!;
            }

            db.Books.Add(book);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveByTagAsync("book", cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Book {BookId} just got created.", book.Id);
            }

            return book.Id;
        }
    }
}
