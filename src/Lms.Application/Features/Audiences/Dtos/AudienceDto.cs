namespace Lms.Application.Features.Audiences.Dtos
{
    public sealed record AudienceDto
    {
        public Guid AudienceId { get; init; }
        public string Name { get; init; } = null!;
    }
}
