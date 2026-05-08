namespace Lms.Application.Features.Users.Dtos
{
    public class TokenDto
    {
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; init; }
        public DateTimeOffset ExpiresOn { get; init; }
    }
}
