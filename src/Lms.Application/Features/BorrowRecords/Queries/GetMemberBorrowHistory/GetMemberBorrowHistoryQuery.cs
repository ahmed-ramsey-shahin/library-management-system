using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.BorrowRecords.Queries.GetMemberBorrowHistory
{
    public sealed record GetMemberBorrowHistoryQuery(Guid MemberId, int PageSize, int Page) : ICachedQuery<Result<PaginatedList<BorrowRecordSummaryDto>>>
    {
        public string CacheKey => $"borrow-record:member:{MemberId}:{PageSize}:{Page}";

        public string[] Tags => ["borrow-record"];

        public TimeSpan Expiration => TimeSpan.FromHours(1);
    }
}
