namespace Lms.Api.Dtos.Requests
{
    public sealed record ChangeUserDetailsRequest(
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Address
    );
}
