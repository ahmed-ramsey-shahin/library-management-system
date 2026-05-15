using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Books.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Books.Queries.GetBookCopyById
{
    public record GetBookCopyByIdQuery(Guid BookCopyId) : ICachedQuery<Result<BookCopyDto>>
    {
        public string CacheKey => $"book-copies:{BookCopyId}";

        public string[] Tags => ["book-copy"];

        public TimeSpan Expiration => TimeSpan.FromHours(1);
    }
}
