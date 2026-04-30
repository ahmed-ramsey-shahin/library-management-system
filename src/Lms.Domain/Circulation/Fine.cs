using Lms.Domain.Common;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;

namespace Lms.Domain.Circulation
{
    public sealed class Fine : AuditableEntity
    {
        public Guid Id { get; }
        public Guid MemberId { get; }
        public User Member { get; private set; } = null!;
        public Guid BorrowRecordId { get; }
        public BorrowRecord BorrowRecord { get; private set; } = null!;
        public FineStatus Status { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public DateTimeOffset FineDate { get; }
        public DateTimeOffset? PaidAt { get; private set; }

        private Fine()
        {}

        private Fine(Guid id, Guid memberId, Guid borrowRecordId, FineStatus status, decimal amount, string description, DateTimeOffset fineDate, DateTimeOffset? paidAt)
        {
            Id = id;
            MemberId = memberId;
            BorrowRecordId = borrowRecordId;
            Status = status;
            Amount = amount;
            Description = description;
            FineDate = fineDate;
            PaidAt = paidAt;
        }

        internal static Result<Fine> Create(
            Guid id,
            Guid memberId,
            Guid borrowRecordId,
            decimal amount,
            string description,
            DateTimeOffset fineDate,
            FineStatus status=FineStatus.Unpaid,
            DateTimeOffset? paidAt=null
        )
        {
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(FineErrors.IdRequired);
            }

            if (memberId == Guid.Empty)
            {
                errors.Add(FineErrors.MemberIdRequired);
            }

            if (borrowRecordId == Guid.Empty)
            {
                errors.Add(FineErrors.BorrowRecordId);
            }

            if (amount <= 0)
            {
                errors.Add(FineErrors.AmountInvalid);
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                errors.Add(FineErrors.DescriptionRequired);
            }

            if (fineDate > DateTimeOffset.UtcNow)
            {
                errors.Add(FineErrors.FineDateInvalid);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new Fine(id, memberId, borrowRecordId, status, amount, description, fineDate, paidAt);
        }

        public Result<Updated> ChangeAmount(decimal amount)
        {
            if (Status != FineStatus.Unpaid)
            {
                return FineErrors.CannotChangeAmount;
            }

            if (amount <= 0)
            {
                return FineErrors.AmountInvalid;
            }

            Amount = amount;
            return Result.Updated;
        }

        public Result<Updated> PayFine()
        {
            if (Status == FineStatus.Paid)
            {
                return Result.Updated;
            }

            Status = FineStatus.Paid;
            PaidAt = DateTimeOffset.UtcNow;
            return Result.Updated;
        }

        public Result<Updated> ChangeFineData(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return FineErrors.DescriptionRequired;
            }

            Description = description;
            return Result.Updated;
        }

        public Result<Deleted> Delete()
        {
            IsDeleted = true;
            return Result.Deleted;
        }

        public Result<Updated> MarkAsWaived()
        {
            if (Status != FineStatus.Unpaid)
            {
                return FineErrors.CannotWaivePaidFines;
            }

            Status = FineStatus.Waived;
            return Result.Updated;
        }
    }
}
