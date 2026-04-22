using Lms.Domain.Catalog;
using Lms.Domain.Common;
using Lms.Domain.Common.Results;
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
            DateOnly pickupDeadline
        )
        {
            Id = id;
            MemberId = memberId;
            BookCopyId = bookCopyId;
            Status = status;
            DueDate = dueDate;
            PickupDeadline = pickupDeadline;
        }

        public static Result<BorrowRecord> Create(
            Guid id,
            Guid memberId,
            Guid bookCopyId,
            BorrowRecordStatus status,
            DateOnly dueDate,
            DateOnly pickupDeadline
        )
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(BorrowRecordErrors.IdRequired);
            }

            if (memberId == Guid.Empty)
            {
                errors.Add(BorrowRecordErrors.MemberIdRequired);
            }

            if (bookCopyId == Guid.Empty)
            {
                errors.Add(BorrowRecordErrors.BookCopyId);
            }

            if (dueDate > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            {
                errors.Add(BorrowRecordErrors.DueDateInvalid);
            }

            if (pickupDeadline > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)))
            {
                errors.Add(BorrowRecordErrors.PickupDeadlineInvalid);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new BorrowRecord(id, memberId, bookCopyId, status, dueDate, pickupDeadline);
        }
    }
}
