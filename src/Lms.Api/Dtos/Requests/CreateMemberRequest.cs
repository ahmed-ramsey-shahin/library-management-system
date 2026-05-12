namespace Lms.Api.Dtos.Requests
{
    public record CreateMemberRequest
    (
        string Email,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Address,
        string Password
    );
}
