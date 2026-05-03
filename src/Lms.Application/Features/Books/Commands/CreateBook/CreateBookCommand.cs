using Lms.Application.Common.Interfaces;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.CreateBook
{
    public sealed record CreateBookCommand : IRequest<Result<Guid>>, IIdempotentCommand
    {
        public string Isbn { get; init; } = null!;
        public string Issn { get; init; } = null!;
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public int PageCount { get; init; }
        public Guid PublisherId { get; init; }
        public DateOnly PublishingDate { get; init; }
        public string Language { get; init; } = null!;
        public string Edition { get; init; } = null!;
        public decimal BorrowPricePerDay { get; init; }
        public decimal FinePerDay { get; init; }
        public decimal LostFee { get; init; }
        public decimal DamageFee { get; init; }
        public List<Guid> CategoryIds { get; init; } = null!;
        public List<Guid> KeywordIds { get; init; } = null!;
        public List<Guid> ThemeIds { get; init; } = null!;
        public List<Guid> GenreIds { get; init; } = null!;
        public List<Guid> AudienceIds { get; init; } = null!;
        public List<Guid> AuthorIds { get; init; } = null!;
        public string IdempotencyKey { get; init; } = null!;
    }
}
