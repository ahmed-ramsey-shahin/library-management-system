using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Publishers.Dtos;
using Lms.Application.Features.Publishers.Mappers;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Publishers.Queries.GetPublishers
{
    public sealed class GetPublishersQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetPublishersQuery, Result<List<PublisherDto>>>
    {
        public async Task<Result<List<PublisherDto>>> Handle(GetPublishersQuery request, CancellationToken cancellationToken)
        {
            var publishers = await db.Publishers.AsNoTracking().ToListAsync(cancellationToken);
            return publishers.ToDto();
        }
    }
}
