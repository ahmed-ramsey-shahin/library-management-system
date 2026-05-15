using Lms.Domain.Catalog;

namespace Lms.Api.Dtos.Requests
{
    public record CreateBookCopyRequest
    (
        string Barcode,
        BookCopyStatus? Status,
        BookCopyState? State,
        string Location,
        DateOnly? AcquisitionDate
    );
}
