using Lms.Application.Common.Interfaces;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;

namespace Lms.Application.Features.BorrowRecords.Queries.GetBorrowRecordById
{
    public sealed record GetBorrowRecordByIdQuery(
        Guid BorrowRecordId,
        Guid CurrentUserId,
        Role CurrentUserRole
    ) : ICachedQuery<Result<BorrowRecordDto>>
    {
        public string CacheKey => $"{CurrentUserId}:borrow-records:{BorrowRecordId}";

        public string[] Tags => ["borrow-record"];

        public TimeSpan Expiration => TimeSpan.FromHours(1);
    }
}
