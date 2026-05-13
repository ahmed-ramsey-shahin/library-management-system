namespace Lms.Api.Dtos.Requests
{
    public record ChangeUserPasswordRequest
    (
        string OldPassword,
        string NewPassword
    );
}
