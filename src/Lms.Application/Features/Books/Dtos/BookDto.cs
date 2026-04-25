using Lms.Application.Features.Audiences.Dtos;
using Lms.Application.Features.Authors.Dtos;
using Lms.Application.Features.Categories.Dtos;
using Lms.Application.Features.Genres.Dtos;
using Lms.Application.Features.Keywords.Dtos;
using Lms.Application.Features.Publishers.Dtos;
using Lms.Application.Features.Themes.Dtos;
namespace Lms.Application.Features.Books.Dtos
{
    public sealed record BookDto
    {
        public Guid BookId { get; init; }
        public string Isbn { get; init; } = null!;
        public string Issn { get; init; } = null!;
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public int PageCount { get; init; }
        public PublisherDto Publisher { get; init; } = null!;
        public DateOnly PublishingDate { get; init; }
        public string Language { get; init; } = null!;
        public string Edition { get; init; } = null!;
        public decimal BorrowPricePerDay { get; init; }
        public decimal FinePerDay { get; init; }
        public decimal LostFee { get; init; }
        public decimal DamageFee { get; init; }
        public int AvailableCopies { get; init; }
        public List<CategoryDto> Categories { get; init; } = null!;
        public List<KeywordDto> Keywords { get; init; } = null!;
        public List<ThemeDto> Themes { get; init; } = null!;
        public List<GenreDto> Genres { get; init; } = null!;
        public List<AudienceDto> Audiences { get; init; } = null!;
        public List<AuthorDto> Authors { get; init; } = null!;
    }
}
