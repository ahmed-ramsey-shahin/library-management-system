namespace Lms.Application.Features.Themes.Dtos
{
    public sealed record ThemeDto
    {
        public Guid ThemeId { get; init; }
        public string Name { get; init; } = null!;
    }
}
