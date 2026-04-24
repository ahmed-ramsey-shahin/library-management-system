using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Authors.Dtos;
using Lms.Application.Features.Authors.Mappers;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Authors.Queries.GetAuthors
{
    public sealed class GetAuthorQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetAuthorsQuery, Result<List<AuthorDto>>>
    {
        public async Task<Result<List<AuthorDto>>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
        {
            var authors = await db.Authors.AsNoTracking().ToListAsync(cancellationToken);
            return authors.ToDto();
        }
    }
}
