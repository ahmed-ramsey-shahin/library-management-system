using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Books.Dtos;
using Lms.Application.Features.Categories.Dtos;
using Lms.Application.Features.Publishers.Dtos;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Lms.Application.Features.Audiences.Dtos;
using Lms.Application.Features.Authors.Dtos;
using Lms.Application.Features.Genres.Dtos;
using Lms.Application.Features.Keywords.Dtos;
using Lms.Application.Features.Themes.Dtos;

namespace Lms.Application.Features.Books.Queries.GetBookById
{
    public sealed class GetBookByIdQueryHandler(
        IAppDbContext db,
        ILogger<GetBookByIdQueryHandler> logger
    ) : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
    {
        public async Task<Result<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await db.Books
                .AsNoTracking()
                .Where(book => book.Id == request.BookId)
                .Select(book => new BookDto
                {
                    BookId = book.Id,
                    Isbn = book.Isbn,
                    Issn = book.Issn,
                    Title = book.Title,
                    Description = book.Description,
                    PageCount = book.PageCount,
                    Publisher = new PublisherDto
                    {
                        PublisherId = book.PublisherId,
                        Name = book.Publisher.Name,
                    },
                    Language = book.Language,
                    Edition = book.Edition,
                    BorrowPricePerDay = book.BorrowPricePerDay,
                    FinePerDay = book.FinePerDay,
                    LostFee = book.LostFee,
                    DamageFee = book.DamageFee,
                    AvailableCopies = book.BookCopies.Count(copy => copy.State == BookCopyState.Available),
                    Categories = book.BookCategories.Select(bookCategory => new CategoryDto{ CategoryId=bookCategory.CategoryId, Name=bookCategory.Category.Name }).ToList(),
                    Keywords = book.BookKeywords.Select(bookKeyword => new KeywordDto{ KeywordId=bookKeyword.KeywordId, Name=bookKeyword.Keyword.Name }).ToList(),
                    Themes = book.BookThemes.Select(bookTheme => new ThemeDto{ ThemeId=bookTheme.ThemeId, Name=bookTheme.Theme.Name }).ToList(),
                    Genres = book.BookGenres.Select(bookGenre => new GenreDto{ GenreId=bookGenre.GenreId, Name=bookGenre.Genre.Name }).ToList(),
                    Audiences = book.BookAudiences.Select(bookAudience => new AudienceDto{ AudienceId=bookAudience.AudienceId, Name=bookAudience.Audience.Name }).ToList(),
                    Authors = book.BookAuthors.Select(bookAuthor => new AuthorDto{ AuthorId=bookAuthor.AuthorId, Name=bookAuthor.Author.Name }).ToList(),
                }).AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

            if (book is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Book query aborted. No book was found with ID {BookId}.", request.BookId);
                }
                return ApplicationErrors.BookNotFound;
            }

            return book;
        }
    }
}
