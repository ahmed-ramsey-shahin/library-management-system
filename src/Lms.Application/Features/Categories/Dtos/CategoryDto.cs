namespace Lms.Application.Features.Categories.Dtos
{
    public sealed record CategoryDto
    {
        public Guid CategoryId { get; init; }
        public string Name { get; init; } = null!;
    }
}
