using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Fines.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Fines.Queries.GetFinesByBorrowRecordId
{
    public sealed record GetFinesByBorrowRecordIdQuery(
        Guid BorrowRecordId,
        int Page,
        int PageSize
    ) : ICachedQuery<Result<PaginatedList<FineDto>>>
    {
        public string CacheKey => $"fines:{BorrowRecordId}:{PageSize}:{Page}";

        public string[] Tags => ["fine"];

        public TimeSpan Expiration => TimeSpan.FromHours(10);
    }
}
