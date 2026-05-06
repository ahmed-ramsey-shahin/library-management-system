using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.BorrowRecords.Queries.GetMemberActiveBorrowings
{
    public sealed record GetMemberActiveBorrowingsQuery(Guid MemberId, int PageSize, int Page) : ICachedQuery<Result<PaginatedList<BorrowRecordSummaryDto>>>
    {
        public string CacheKey => $"borrow-records:{MemberId}:active:{PageSize}:{Page}";

        public string[] Tags => ["borrow-record"];

        public TimeSpan Expiration => TimeSpan.FromHours(1);
    }
}
