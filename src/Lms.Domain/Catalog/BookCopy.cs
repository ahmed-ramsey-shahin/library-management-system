using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public sealed class BookCopy : AuditableEntity
    {
        public Guid Id { get; }
        public Guid BookId { get; }
        public string Barcode { get; private set; } = string.Empty;
        public BookCopyStatus Status { get; private set; } = BookCopyStatus.Good;
        public BookCopyState State { get; private set; } = BookCopyState.Available;
        public string Location { get; private set; } = string.Empty;
        public DateOnly AcquisitionDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public byte[] Version { get; } = null!;

        // readonly navigation
        public Book Book { get; } = null!;

        private BookCopy()
        {}

        private BookCopy(Guid id, Guid bookId, string barcode, BookCopyStatus status, BookCopyState state, string location, DateOnly acquisitionDate)
        {
            Id = id;
            BookId = bookId;
            Barcode = barcode;
            Status = status;
            State = state;
            Location = location;
            AcquisitionDate = acquisitionDate;
        }

        internal static Result<BookCopy> Create(
            Guid id,
            Guid bookId,
            string barcode,
            string location,
            DateOnly? acquisitionDate,
            BookCopyStatus status=BookCopyStatus.Good,
            BookCopyState state=BookCopyState.Available
        )
        {
            acquisitionDate ??= DateOnly.FromDateTime(DateTime.UtcNow);
            List<Error> errors = [];

            if (id == Guid.Empty)
            {
                errors.Add(BookCopyErrors.IdRequired);
            }

            if (bookId == Guid.Empty)
            {
                errors.Add(BookCopyErrors.BookIdRequired);
            }

            if (string.IsNullOrWhiteSpace(barcode))
            {
                errors.Add(BookCopyErrors.BarcodeRequired);
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                errors.Add(BookCopyErrors.LocationRequired);
            }

            if (acquisitionDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                errors.Add(BookCopyErrors.AcquisitionDateInvalid);
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            return new BookCopy(id, bookId, barcode, status, state, location, acquisitionDate.Value);
        }

        internal Result<Deleted> Delete()
        {
            if (State == BookCopyState.Borrowed)
            {
                return BookCopyErrors.CannotDeleteBorrowedCopy;
            }

            IsDeleted = true;
            return Result.Deleted;
        }

        internal Result<Updated> ChangeStatus(BookCopyStatus status)
        {
            if (State == BookCopyState.Borrowed)
            {
                return BookCopyErrors.CantChangeStateOfBorrowedBook;
            }

            Status = status;
            return Result.Updated;
        }

        internal Result<Updated> MarkAsWaitingApproval()
        {
            if (Status != BookCopyStatus.Good)
            {
                return BookCopyErrors.CopyNotGood;
            }

            if (State != BookCopyState.Available)
            {
                return BookCopyErrors.CantChangeStateOfBorrowedBook;
            }

            if (State == BookCopyState.WaitingApproval)
            {
                return Result.Updated;
            }

            State = BookCopyState.WaitingApproval;
            return Result.Updated;
        }

        internal Result<Updated> MarkAsBorrowed()
        {
            if (Status != BookCopyStatus.Good)
            {
                return BookCopyErrors.CopyNotGood;
            }

            if (State == BookCopyState.Borrowed)
            {
                return Result.Updated;
            }

            State = BookCopyState.Borrowed;
            return Result.Updated;
        }

        internal Result<Updated> MarkAsAvailable()
        {
            State = BookCopyState.Available;
            return Result.Updated;
        }

        internal Result<Updated> MarkAsMaintenance()
        {
            State = BookCopyState.Maintenance;
            return Result.Updated;
        }

        internal Result<Updated> ChangeLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BookCopyErrors.LocationRequired;
            }

            Location = location;
            return Result.Updated;
        }
    }
}
