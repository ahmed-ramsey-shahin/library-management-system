using Lms.Domain.Common.Results;

namespace Lms.Domain.Identity
{
    public static class RefreshTokenErrors
    {
        public static Error IdRequired => Error.Validation("RefreshToken.Id.Required", "Refresh token id is required.");
        public static Error TokenRequired => Error.Validation("RefreshToken.Token.Required", "Refresh token is required.");
        public static Error UserIdRequired => Error.Validation("RefreshToken.UserId.Required", "The id of user associated with this refresh token is required.");
        public static Error ExpirationInvalid => Error.Validation("RefreshToken.ExpiresOn.Invalid", "The expiration date of the refresh token must be in the future.");
    }
}
