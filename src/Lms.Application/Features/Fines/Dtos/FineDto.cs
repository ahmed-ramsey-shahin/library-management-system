using Lms.Domain.Circulation;

namespace Lms.Application.Features.Fines.Dtos
{
    public sealed record FineDto
    {
        public Guid FineId { get; init; }
        public Guid MemberId { get; init; }
        public string MemberName { get; init; } = null!;
        public Guid BorrowRecordId { get; init; }
        public string BookTitle { get; init; } = null!;
        public FineStatus Status { get; init; }
        public decimal Amount { get; init; }
        public string Description { get; init; } = null!;
        public DateTimeOffset FineDate { get; init; }
        public DateTimeOffset? PaidAt { get; init; }
    }
}
