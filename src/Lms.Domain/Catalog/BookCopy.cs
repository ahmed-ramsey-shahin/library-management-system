using Lms.Domain.Circulation;
using Lms.Domain.Common;
using Lms.Domain.Common.Results;

namespace Lms.Domain.Catalog
{
    public sealed class BookCopy : AuditableEntity
    {
        public Guid Id { get; }
        public Guid BookId { get; private set; }
        public Book Book { get; private set; } = null!;
        public string Barcode { get; private set; } = string.Empty;
        public BookCopyStatus Status { get; private set; } = BookCopyStatus.Good;
        public BookCopyState State { get; private set; } = BookCopyState.Available;
        public string Location { get; private set; } = string.Empty;
        public DateOnly AcquisitionDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public byte[] Version { get; private set; } = null!;
        private readonly List<BorrowRecord> _borrowRecords = [];
        public IReadOnlyCollection<BorrowRecord> BorrowRecords => _borrowRecords.AsReadOnly();

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
            return new BookCopy(id, bookId, barcode, status, state, location, acquisitionDate.Value);
        }
    }
}
