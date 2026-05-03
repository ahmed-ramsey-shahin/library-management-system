using Lms.Domain.Identity;

namespace Lms.Application.Features.Users.Dtos
{
    public sealed record UserDto
    {
        public Guid UserId { get; init; }
        public string Email { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string LibraryCardNumber { get; init; } = null!;
        public UserStatus Status { get; init; }
        public string PasswordHash { get; init; } = null!;
        public Role Role { get; init; }
    }
}
