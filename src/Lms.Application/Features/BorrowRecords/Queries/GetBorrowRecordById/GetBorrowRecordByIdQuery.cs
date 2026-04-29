using Lms.Application.Common.Interfaces;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.BorrowRecords.Queries.GetBorrowRecordById
{
    public sealed record GetBorrowRecordByIdQuery(
        Guid BorrowRecordId
    ) : ICachedQuery<Result<BorrowRecordDto>>
    {
        public string CacheKey => $"borrow-records:{BorrowRecordId}";

        public string[] Tags => ["borrow-record"];

        public TimeSpan Expiration => TimeSpan.FromHours(1);
    }
}
