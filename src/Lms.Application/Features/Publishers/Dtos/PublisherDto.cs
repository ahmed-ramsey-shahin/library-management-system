namespace Lms.Application.Features.Publishers.Dtos
{
    public sealed record PublisherDto
    {
        public Guid PublisherId { get; init; }
        public string Name { get; init; } = null!;
    }
}
