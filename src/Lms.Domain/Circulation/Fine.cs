using Lms.Domain.Common;
using Lms.Domain.Identity;

namespace Lms.Domain.Circulation
{
    public sealed class Fine : AuditableEntity
    {
        public Guid Id { get; }
        public Guid MemberId { get; private set; }
        public User Member { get; private set; } = null!;
        public Guid BorrowRecordId { get; private set; }
        public BorrowRecord BorrowRecord { get; private set; } = null!;
        public FineStatus Status { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public DateTimeOffset FineDate { get; private set; }

        private Fine()
        {}

        private Fine(Guid id, Guid memberId, Guid borrowRecordId, FineStatus status, decimal amount, string description, DateTimeOffset fineDate)
        {
            Id = id;
            MemberId = memberId;
            BorrowRecordId = borrowRecordId;
            Status = status;
            Amount = amount;
            Description = description;
            FineDate = fineDate;
        }
    }
}
