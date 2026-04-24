using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Genres.Dtos;
using Lms.Application.Features.Genres.Mappers;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Genres.Queries.GetGenres
{
    public sealed class GetGenresQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetGenresQuery, Result<List<GenreDto>>>
    {
        public async Task<Result<List<GenreDto>>> Handle(GetGenresQuery request, CancellationToken cancellationToken)
        {
            var genres = await db.Genres.AsNoTracking().ToListAsync(cancellationToken);
            return genres.ToDto();
        }
    }
}
