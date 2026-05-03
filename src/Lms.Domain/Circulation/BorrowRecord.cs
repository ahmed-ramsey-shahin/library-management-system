using Lms.Domain.Catalog;
using Lms.Domain.Common;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;

namespace Lms.Domain.Circulation
{
    public sealed class BorrowRecord : EventfulEntity
    {
        public Guid Id { get; }
        public Guid MemberId { get; }
        public User Member { get; } = null!;
        public Guid BookCopyId { get; }
        public BookCopy BookCopy { get; } = null!;
        public BorrowRecordStatus Status { get; private set; } = BorrowRecordStatus.Waiting;
        public DateOnly DueDate { get; private set; }
        public decimal BorrowingCost { get; private set; }
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
            DateOnly pickupDeadline,
            decimal borrowingCost
        )
        {
            Id = id;
            MemberId = memberId;
            BookCopyId = bookCopyId;
            Status = status;
            DueDate = dueDate;
            PickupDeadline = pickupDeadline;
            BorrowingCost = borrowingCost;
        }

        public static Result<BorrowRecord> Create(
            Guid id,
            Guid memberId,
            Guid bookCopyId,
            DateOnly dueDate,
            DateOnly pickupDeadline,
            decimal borrowingCost,
            BorrowRecordStatus status=BorrowRecordStatus.Waiting
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

            if (dueDate <= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)))
            {
                errors.Add(BorrowRecordErrors.DueDateLessThanWeek);
            }

            if (pickupDeadline <= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
            {
                errors.Add(BorrowRecordErrors.PickupDeadlineLessThanDay);
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

            return new BorrowRecord(id, memberId, bookCopyId, status, dueDate, pickupDeadline, borrowingCost);
        }

        public Result<IReadOnlyCollection<Fine>> GetUnpaidFines()
        {
            return Fines.Where(fine => fine.Status == FineStatus.Unpaid).ToList();
        }

        public Result<Fine> AddFine(
            Guid id,
            decimal amount,
            string description,
            DateTimeOffset? fineDate=null,
            FineStatus status=FineStatus.Unpaid,
            DateTimeOffset? paidAt=null
        )
        {
            var today = DateTimeOffset.UtcNow.Date;
            var fine = _fines.FirstOrDefault(fine => fine.Id == id);

            if (fine is not null)
            {
                return BorrowRecordErrors.FineAlreadyExists;
            }

            var alreadyFinedToday = _fines.Any(fine => fine.FineDate.Date == today);

            if (alreadyFinedToday)
            {
                return BorrowRecordErrors.DailyFineAlreadyAssessed;
            }

            fineDate ??= DateTimeOffset.UtcNow;
            var fineResult = Fine.Create(id, MemberId, Id, amount, description, fineDate.Value, status, paidAt);

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

        public Result<Updated> AcceptBorrowRequest()
        {
            if (Status == BorrowRecordStatus.Accepted)
            {
                return Result.Updated;
            }

            if (Status != BorrowRecordStatus.Waiting)
            {
                return BorrowRecordErrors.ResponseInvalid(Status);
            }

            Status = BorrowRecordStatus.Accepted;
            AddEvent(new BorrowRequestAcceptedEvent(Id));
            return Result.Updated;
        }

        public Result<Updated> Return()
        {
            if (Status == BorrowRecordStatus.Returned)
            {
                return Result.Updated;
            }

            if (Status == BorrowRecordStatus.Accepted || Status == BorrowRecordStatus.Late)
            {
                Status = BorrowRecordStatus.Returned;
                AddEvent(new BookReturnedEvent(Id));
                return Result.Updated;
            }

            return BorrowRecordErrors.ReturnInvalid(Status);
        }

        public Result<Updated> Renew(decimal renewalCost, DateOnly newDueDate)
        {
            if (Status != BorrowRecordStatus.Accepted)
            {
                return BorrowRecordErrors.RenewInvalid(Status);
            }

            if (newDueDate <= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)))
            {
                return BorrowRecordErrors.DueDateLessThanWeek;
            }

            if (newDueDate > DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)))
            {
                return BorrowRecordErrors.DueDateInvalid;
            }

            RenewalCount++;
            DueDate = newDueDate;
            BorrowingCost += renewalCost;
            AddEvent(new BookRenewedEvent(Id));
            return Result.Updated;
        }

        public Result<Updated> RejectBorrowRequest()
        {
            if (Status == BorrowRecordStatus.Rejected)
            {
                return Result.Updated;
            }

            if (Status != BorrowRecordStatus.Waiting)
            {
                return BorrowRecordErrors.RejectInvalid(Status);
            }

            Status = BorrowRecordStatus.Rejected;
            AddEvent(new BorrowRequestRejectedEvent(Id));
            return Result.Updated;
        }

        public Result<Updated> PayFine(Guid fineId)
        {
            if (Status != BorrowRecordStatus.Accepted && Status != BorrowRecordStatus.Late && Status != BorrowRecordStatus.Returned)
            {
                return BorrowRecordErrors.PayFineInvalid;
            }

            if (fineId == Guid.Empty)
            {
                return BorrowRecordErrors.FineNotFound;
            }

            var fine = _fines.FirstOrDefault(fine => fine.Id == fineId);

            if (fine is null)
            {
                return BorrowRecordErrors.FineNotFound;
            }

            var payFineResult = fine.PayFine();

            if (payFineResult.IsError)
            {
                return payFineResult.Errors!;
            }

            AddEvent(new FinePaidEvent(fineId, Id));
            return Result.Updated;
        }

        public Result<Updated> PayAllFines()
        {
            foreach (var fine in _fines.Where(fine => fine.Status == FineStatus.Unpaid).ToList())
            {
                var payFineResult = PayFine(fine.Id);
                if (payFineResult.IsError)
                {
                    return payFineResult.Errors!;
                }
            }

            return Result.Updated;
        }

        public Result<Updated> MarkAsLate()
        {
            if (Status == BorrowRecordStatus.Late)
            {
                return Result.Updated;
            }

            if (Status != BorrowRecordStatus.Accepted)
            {
                return BorrowRecordErrors.CannotMarkAsLate;
            }

            Status = BorrowRecordStatus.Late;
            AddEvent(new BorrowRecordMarkedAsLateEvent(Id));
            return Result.Updated;
        }

        public Result<Deleted> Cancel()
        {
            if (Status != BorrowRecordStatus.Waiting)
            {
                return BorrowRecordErrors.CancellationInvalid(Status);
            }

            Status = BorrowRecordStatus.Canceled;
            return Result.Deleted;
        }

        public Result<Updated> ChangeFineAmount(Guid fineId, decimal amount)
        {
            if (Status != BorrowRecordStatus.Accepted && Status != BorrowRecordStatus.Late && Status != BorrowRecordStatus.Returned)
            {
                return BorrowRecordErrors.PayFineInvalid;
            }

            if (fineId == Guid.Empty)
            {
                return BorrowRecordErrors.FineNotFound;
            }

            var fine = _fines.FirstOrDefault(fine => fine.Id == fineId);

            if (fine is null)
            {
                return BorrowRecordErrors.FineNotFound;
            }

            var updateResult = fine.ChangeAmount(amount);

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            return Result.Updated;
        }

        public Result<Updated> MarkFineWaived(Guid fineId)
        {
            if (fineId == Guid.Empty)
            {
                return BorrowRecordErrors.FineNotFound;
            }

            var fine = _fines.FirstOrDefault(fine => fine.Id == fineId);

            if (fine is null)
            {
                return BorrowRecordErrors.FineNotFound;
            }

            var updateResult = fine.MarkAsWaived();

            if (updateResult.IsError)
            {
                return updateResult.Errors!;
            }

            return Result.Updated;
        }
    }
}
