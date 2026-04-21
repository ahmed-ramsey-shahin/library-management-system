using Lms.Domain.Catalog;
using Lms.Domain.Common;
using Lms.Domain.Identity;

namespace Lms.Domain.Circulation
{
    public sealed class BorrowRecord : AuditableEntity
    {
        public Guid Id { get; }
        public Guid MemberId { get; private set; }
        public User Member { get; private set; } = null!;
        public Guid BookCopyId { get; private set; }
        public BookCopy BookCopy { get; private set; } = null!;
        public BorrowRecordStatus Status { get; private set; } = BorrowRecordStatus.Waiting;
        public DateOnly DueDate { get; private set; }
        public decimal FineAccrued { get; private set; }
        public int RenewalCount { get; private set; }
        public DateOnly PickupDeadline { get; private set; }
        private readonly List<Fine> _fines = [];
        public IReadOnlyCollection<Fine> Fines => _fines.AsReadOnly();

        private BorrowRecord()
        {}

        private BorrowRecord(
            Guid id,
            Guid memberId,
            Guid bookCopyId,
            BorrowRecordStatus status,
            DateOnly dueDate,
            decimal fineAccrued,
            int renewalCount,
            DateOnly pickupDeadline
        )
        {
            Id = id;
            MemberId = memberId;
            BookCopyId = bookCopyId;
            Status = status;
            DueDate = dueDate;
            FineAccrued = fineAccrued;
            RenewalCount = renewalCount;
            PickupDeadline = pickupDeadline;
        }
    }
}
