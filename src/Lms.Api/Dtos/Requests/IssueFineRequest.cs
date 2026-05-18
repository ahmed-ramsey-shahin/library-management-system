namespace Lms.Api.Dtos.Requests
{
    public record IssueFineRequest
    (
        Guid BorrowRecordId,
        decimal Amount,
        string Description
    );
}
