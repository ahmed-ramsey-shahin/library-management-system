using Lms.Domain.Circulation;

namespace Lms.Application.Features.BorrowRecords.Dto
{
    public sealed record BorrowRecordDto
    {
        public Guid BorrowRecordId { get; init; }
        public Guid MemberId { get; init; }
        public Guid BookCopyId { get; init; }
        public Guid BookId { get; init; }

        public BorrowRecordStatus Status { get; init; }
        public DateOnly DueDate { get; init; }
        public DateOnly PickupDeadline { get; init; }
        public decimal BorrowingCost { get; init; }
        public int RenewalCount { get; init; }
    }
}
