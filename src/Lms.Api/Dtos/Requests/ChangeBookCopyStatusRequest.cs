using Lms.Domain.Catalog;

namespace Lms.Api.Dtos.Requests
{
    public record ChangeBookCopyStatusRequest(BookCopyStatus Status, byte[] Version);
}
