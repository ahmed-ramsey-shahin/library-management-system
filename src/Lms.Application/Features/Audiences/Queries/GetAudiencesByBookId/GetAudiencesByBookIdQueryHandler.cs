using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Audiences.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Audiences.Queries.GetAudiencesByBookId
{
    public sealed class GetAudiencesByBookIdQueryHandler(
        IAppDbContext db,
        ILogger<GetAudiencesByBookIdQueryHandler> logger
    ) : IRequestHandler<GetAudiencesByBookIdQuery, Result<List<AudienceDto>>>
    {
        public async Task<Result<List<AudienceDto>>> Handle(GetAudiencesByBookIdQuery request, CancellationToken cancellationToken)
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

            return await db.Audiences
                .Join(db.BookAudiences, keywords => keywords.Id, bk => bk.AudienceId, (keyword, book) => new
                {
                    AudienceId = keyword.Id,
                    AudienceName = keyword.Name,
                    book.BookId,
                }).Where(keyword => keyword.BookId == request.BookId)
                .Select(keyword => new AudienceDto
                {
                    AudienceId = keyword.AudienceId,
                    Name = keyword.AudienceName,
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
