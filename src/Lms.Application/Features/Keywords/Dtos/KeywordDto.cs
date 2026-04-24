namespace Lms.Application.Features.Keywords.Dtos
{
    public sealed record KeywordDto
    {
        public Guid KeywordId { get; init; }
        public string Name { get; init; } = null!;
    }
}
