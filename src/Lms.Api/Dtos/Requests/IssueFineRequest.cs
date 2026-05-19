namespace Lms.Api.Dtos.Requests
{
    public record IssueFineRequest
    (
        decimal Amount,
        string Description
    );
}
