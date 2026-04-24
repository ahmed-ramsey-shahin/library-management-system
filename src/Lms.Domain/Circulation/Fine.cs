using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation
{
    public sealed class Fine : AuditableEntity
    {
        public Guid Id { get; }
        public Guid MemberId { get; }
        public Guid BorrowRecordId { get; }
        public FineStatus Status { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public DateTimeOffset FineDate { get; }

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

        internal static Result<Fine> Create(
            Guid id,
            Guid memberId,
            Guid borrowRecordId,
            decimal amount,
            string description,
            DateTimeOffset fineDate,
            FineStatus status=FineStatus.Unpaid
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

            return new Fine(id, memberId, borrowRecordId, status, amount, description, fineDate);
        }

        public Result<Updated> ChangeAmount(decimal amount)
        {
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
                return FineErrors.FineAlreadyPaid;
            }

            Status = FineStatus.Paid;
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
            if (Status == FineStatus.Unpaid)
            {
                return FineErrors.FineUnpaid;
            }

            IsDeleted = true;
            return Result.Deleted;
        }
    }
}
