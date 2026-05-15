using Lms.Domain.Catalog;

namespace Lms.Application.Features.Books.Dtos
{
    public sealed record BookCopySummaryDto
    {
        public string Barcode { get; init; } = null!;
        public BookCopyState State { get; init; }
        public BookCopyStatus Status { get; init; }
        public string Location { get; init; } = null!;
        public DateOnly AcquisitionDate { get; init; }
        public byte[] Version { get; init; } = null!;
    }
}
