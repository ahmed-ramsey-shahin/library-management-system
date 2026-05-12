namespace Lms.Api.Dtos.Requests
{
    public record CreateLibrarianRequest
    (
        string Email,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Address,
        string Password,
        List<Guid> CategoryIds
    );
}
