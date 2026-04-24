using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Authors.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Authors.Queries.GetAuthorsByBookId
{
    public sealed class GetAuthorsByBookIdQueryHandler(
        IAppDbContext db,
        ILogger<GetAuthorsByBookIdQueryHandler> logger
    ) : IRequestHandler<GetAuthorsByBookIdQuery, Result<List<AuthorDto>>>
    {
        public async Task<Result<List<AuthorDto>>> Handle(GetAuthorsByBookIdQuery request, CancellationToken cancellationToken)
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

            return await db.Authors
                .Join(db.BookAuthors, authors => authors.Id, bg => bg.AuthorId, (author, book) => new
                {
                    AuthorId = author.Id,
                    AuthorName = author.Name,
                    book.BookId,
                }).Where(author => author.BookId == request.BookId)
                .Select(author => new AuthorDto
                {
                    AuthorId = author.AuthorId,
                    Name = author.AuthorName,
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
