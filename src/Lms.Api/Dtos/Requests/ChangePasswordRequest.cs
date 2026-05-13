namespace Lms.Api.Dtos.Requests
{
    public record ChangePasswordRequest
    (
        string OldPassword,
        string NewPassword
    );
}
