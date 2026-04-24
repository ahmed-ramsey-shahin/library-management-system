using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Themes.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Themes.Queries.GetThemesByBookId
{
    public sealed class GetThemesByBookIdQueryHandler(
        IAppDbContext db,
        ILogger<GetThemesByBookIdQueryHandler> logger
    ) : IRequestHandler<GetThemesByBookIdQuery, Result<List<ThemeDto>>>
    {
        public async Task<Result<List<ThemeDto>>> Handle(GetThemesByBookIdQuery request, CancellationToken cancellationToken)
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

            return await db.Themes
                .Join(db.BookThemes, themes => themes.Id, bg => bg.ThemeId, (theme, book) => new
                {
                    ThemeId = theme.Id,
                    ThemeName = theme.Name,
                    book.BookId,
                }).Where(theme => theme.BookId == request.BookId)
                .Select(theme => new ThemeDto
                {
                    ThemeId = theme.ThemeId,
                    Name = theme.ThemeName,
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
