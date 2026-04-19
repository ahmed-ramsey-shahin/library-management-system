using Lms.Domain.Common.Results;

namespace Lms.Domain.Identity
{
    public sealed class RefreshToken
    {
        public Guid Id { get; }
        public string Token { get; } = string.Empty;
        public Guid UserId { get; }
        public DateTimeOffset ExpiresOn { get; }

        private RefreshToken()
        {}

        private RefreshToken(Guid id, string token, Guid userId, DateTimeOffset expiresOn)
        {
            Id = id;
            Token = token;
            UserId = userId;
            ExpiresOn = expiresOn;
        }

        public static Result<RefreshToken> Create(Guid id, string token, Guid userId, DateTimeOffset expiresOn)
        {
            if (id == Guid.Empty)
            {
                return RefreshTokenErrors.IdRequired;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return RefreshTokenErrors.TokenRequired;
            }

            if (userId == Guid.Empty)
            {
                return RefreshTokenErrors.UserIdRequired;
            }

            if (expiresOn <= DateTimeOffset.UtcNow)
            {
                return RefreshTokenErrors.ExpirationInvalid;
            }

            return new RefreshToken(id, token, userId, expiresOn);
        }
    }
}
