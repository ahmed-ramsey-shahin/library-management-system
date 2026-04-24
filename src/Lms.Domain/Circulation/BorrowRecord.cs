using Lms.Domain.Common;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;

namespace Lms.Domain.Circulation
{
    public sealed class BorrowRecord : AuditableEntity
    {
        public Guid Id { get; }
        public Guid MemberId { get; }
        public Guid BookCopyId { get; }
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

        public Result<IReadOnlyCollection<Fine>> GetUnpaidFines()
        {
            return Fines.Where(fine => fine.Status == FineStatus.Unpaid).ToList();
        }

        public Result<Fine> AddFine(
            Guid id,
            decimal amount,
            string description,
            DateTimeOffset? fineDate=null
        )
        {
            var fine = _fines.FirstOrDefault(fine => fine.Id == id);

            if (fine is not null)
            {
                return BorrowRecordErrors.FineAlreadyExists;
            }

            fineDate ??= DateTimeOffset.UtcNow;
            var fineResult = Fine.Create(id, MemberId, Id, amount, description, fineDate.Value);

            if (fineResult.IsError)
            {
                return fineResult.Errors!;
            }

            _fines.Add(fineResult.Value);
            return fineResult.Value;
        }

        public Result<Updated> RemoveFine(Guid id)
        {
            if (id == Guid.Empty)
            {
                return FineErrors.IdRequired;
            }

            var fine = _fines.FirstOrDefault(fine => fine.Id == id);
            if (fine is null)
            {
                return BorrowRecordErrors.FineNotFound;
            }

            var result = fine.Delete();

            if (result.IsError)
            {
                return result.Errors!;
            }

            return Result.Updated;
        }

        public Result<Updated> AcceptBorrowRequest(User member, int activeBorrows, int maxActiveBorrows, int unpaidFines, int maxUnpaidFine, int lateBorrows, int maxLateBorrows)
        {
            if (Status == BorrowRecordStatus.Accepted)
            {
                return Result.Updated;
            }

            if (Status != BorrowRecordStatus.Waiting)
            {
                return BorrowRecordErrors.ResponseInvalid(Status);
            }

            var canBorrow = member.CanBorrow(activeBorrows, maxActiveBorrows, unpaidFines, maxUnpaidFine, lateBorrows, maxLateBorrows);

            if (canBorrow.IsError)
            {
                return canBorrow.Errors!;
            }

            if (canBorrow.Value)
            {
                return BorrowRecordErrors.MemberNotApplicable;
            }

            Status = BorrowRecordStatus.Accepted;
            return Result.Updated;
        }
    }
}
