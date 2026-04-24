using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Themes.Dtos;
using Lms.Application.Features.Themes.Mappers;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Themes.Queries.GetThemes
{
    public sealed class GetThemesQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetThemesQuery, Result<List<ThemeDto>>>
    {
        public async Task<Result<List<ThemeDto>>> Handle(GetThemesQuery request, CancellationToken cancellationToken)
        {
            var themes = await db.Themes.AsNoTracking().ToListAsync(cancellationToken);
            return themes.ToDto();
        }
    }
}
