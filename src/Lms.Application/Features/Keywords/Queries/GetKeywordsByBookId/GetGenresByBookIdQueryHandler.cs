using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Keywords.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Keywords.Queries.GetKeywordsByBookId
{
    public sealed class GetKeywordsByBookIdQueryHandler(
        IAppDbContext db,
        ILogger<GetKeywordsByBookIdQueryHandler> logger
    ) : IRequestHandler<GetKeywordsByBookIdQuery, Result<List<KeywordDto>>>
    {
        public async Task<Result<List<KeywordDto>>> Handle(GetKeywordsByBookIdQuery request, CancellationToken cancellationToken)
        {
            var bookExists = await db.Books.AnyAsync(book => book.Id == request.BookId, cancellationToken);

            if (!bookExists)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Could not find book with ID {BookId}", request.BookId);
                }

                return ApplicationErrors.BookNotFound;
            }

            return await db.Keywords
                .Join(db.BookKeywords, keywords => keywords.Id, bk => bk.KeywordId, (keyword, book) => new
                {
                    KeywordId = keyword.Id,
                    KeywordName = keyword.Name,
                    book.BookId,
                }).Where(keyword => keyword.BookId == request.BookId)
                .Select(keyword => new KeywordDto
                {
                    KeywordId = keyword.KeywordId,
                    Name = keyword.KeywordName,
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
