namespace Lms.Application.Features.Genres.Dtos
{
    public sealed record GenreDto
    {
        public Guid GenreId { get; init; }
        public string Name { get; init; } = null!;
    }
}
