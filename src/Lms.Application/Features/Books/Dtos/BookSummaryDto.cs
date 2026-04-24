namespace Lms.Application.Features.Books.Dtos
{
    public sealed record BookSummaryDto
    {
        public Guid BookId { get; init; }
        public string Isbn { get; init; } = null!;
        public string Title { get; init; } = null!;
        public string Edition { get; init; } = null!;
        public int AvailableCopies { get; init; }
    }
}
