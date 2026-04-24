namespace Lms.Application.Features.Authors.Dtos
{
    public sealed record AuthorDto
    {
        public Guid AuthorId { get; init; }
        public string Name { get; init; } = null!;
    }
}
