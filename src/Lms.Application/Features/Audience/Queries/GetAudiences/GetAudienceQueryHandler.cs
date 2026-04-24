using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Audiences.Dtos;
using Lms.Application.Features.Audiences.Mappers;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Audiences.Queries.GetAudiences
{
    public sealed class GetAudienceQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetAudiencesQuery, Result<List<AudienceDto>>>
    {
        public async Task<Result<List<AudienceDto>>> Handle(GetAudiencesQuery request, CancellationToken cancellationToken)
        {
            var genres = await db.Audiences.AsNoTracking().ToListAsync(cancellationToken);
            return genres.ToDto();
        }
    }
}
