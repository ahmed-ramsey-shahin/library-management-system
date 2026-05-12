namespace Lms.Api.Dtos.Requests
{
    public record CreateAdminRequest
    (
        string Email,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Address,
        string Password
    );
}
