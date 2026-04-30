using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Fines.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Fines.Queries.GetFineById
{
    public sealed record GetFineByIdQuery(Guid FineId) : ICachedQuery<Result<FineDto>>
    {
        public string CacheKey => $"fines:{FineId}";

        public string[] Tags => ["fine"];

        public TimeSpan Expiration => TimeSpan.FromHours(1);
    }
}
