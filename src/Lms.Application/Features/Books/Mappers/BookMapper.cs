using Lms.Application.Features.Audiences.Dtos;
using Lms.Application.Features.Authors.Dtos;
using Lms.Application.Features.Books.Dtos;
using Lms.Application.Features.Categories.Dtos;
using Lms.Application.Features.Genres.Dtos;
using Lms.Application.Features.Keywords.Dtos;
using Lms.Application.Features.Publishers.Dtos;
using Lms.Application.Features.Themes.Dtos;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Books.Mappers
{
    public static class BookMapper
    {
        public static BookSummaryDto ToSummaryDto(this Book book)
        {
            return new()
            {
                BookId = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                Edition = book.Edition,
                AvailableCopies = book.AvailableCopies,
            };
        }

        public static List<BookSummaryDto> ToSummaryDto(this IEnumerable<Book> books)
        {
            return [.. books.Select(ToSummaryDto)];
        }

        public static BookDto ToDto(
            this Book book,
            PublisherDto publisherDto,
            List<CategoryDto> categoryDtos,
            List<KeywordDto> keywordDtos,
            List<ThemeDto> themeDtos,
            List<GenreDto> genreDtos,
            List<AudienceDto> audienceDtos,
            List<AuthorDto> authorDtos
        )
        {
            return new BookDto
            {
                BookId = book.Id,
                Isbn = book.Isbn,
                Issn = book.Issn,
                Title = book.Title,
                Description = book.Description,
                PageCount = book.PageCount,
                Publisher = publisherDto,
                PublishingDate = book.PublishingDate,
                Language = book.Language,
                Edition = book.Edition,
                BorrowPricePerDay = book.BorrowPricePerDay,
                FinePerDay = book.FinePerDay,
                LostFee = book.LostFee,
                DamageFee = book.DamageFee,
                AvailableCopies = book.AvailableCopies,
                Categories = categoryDtos,
                Keywords = keywordDtos,
                Themes = themeDtos,
                Genres = genreDtos,
                Audiences = audienceDtos,
                Authors = authorDtos,
            };
        }
    }
}
