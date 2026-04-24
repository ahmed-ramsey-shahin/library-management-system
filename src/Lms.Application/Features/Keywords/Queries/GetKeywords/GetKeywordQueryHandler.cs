using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Keywords.Dtos;
using Lms.Application.Features.Keywords.Mappers;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Keywords.Queries.GetKeywords
{
    public sealed class GetKeywordQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetKeywordsQuery, Result<List<KeywordDto>>>
    {
        public async Task<Result<List<KeywordDto>>> Handle(GetKeywordsQuery request, CancellationToken cancellationToken)
        {
            var genres = await db.Keywords.AsNoTracking().ToListAsync(cancellationToken);
            return genres.ToDto();
        }
    }
}
