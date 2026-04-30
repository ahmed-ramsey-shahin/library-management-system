using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Fines.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Fines.Queries.GetUnpaidFines
{
    public sealed record GetUnpaidFinesQuery(int PageSize, int Page) : ICachedQuery<Result<PaginatedList<FineDto>>>
    {
        public string CacheKey => $"fines:unpaid:{PageSize}:{Page}";

        public string[] Tags => ["fine"];

        public TimeSpan Expiration => TimeSpan.FromHours(1);
    }
}
