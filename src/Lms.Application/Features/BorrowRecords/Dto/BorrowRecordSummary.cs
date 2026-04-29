namespace Lms.Application.Features.BorrowRecords.Dto
{
    public sealed record BorrowRecordSummary
    {
        public Guid BorrowRecordId { get; init; }
        public Guid MemberId { get; init; }
        public Guid BookCopyId { get; init; }
        public Guid BookId { get; init; }
        public string BookTitle { get; init; } = null!;
        public string BookCopyLocation { get; init; } = null!;
        public DateOnly DueDate { get; init; }
        public decimal BorrowingCost { get; init; }
        public DateOnly PickupDeadline { get; init; }
    }
}
