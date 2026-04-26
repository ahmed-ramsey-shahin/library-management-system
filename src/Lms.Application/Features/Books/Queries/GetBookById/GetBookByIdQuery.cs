using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Books.Queries.GetBookById
{
    public sealed record GetBookByIdQuery(Guid BookId) : ICachedQuery<Result<BookDto>>
    {
        public string CacheKey => $"books:{BookId}";

        public string[] Tags => ["book", "book-copy"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
