using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Fines.Dtos;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;

namespace Lms.Application.Features.Fines.Queries.GetFineById
{
    public sealed record GetFineByIdQuery(Guid FineId, Guid CurrentUserId, Role CurrentUserRole) : ICachedQuery<Result<FineDto>>
    {
        public string CacheKey => $"{CurrentUserId}:fines:{FineId}";

        public string[] Tags => ["fine"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
