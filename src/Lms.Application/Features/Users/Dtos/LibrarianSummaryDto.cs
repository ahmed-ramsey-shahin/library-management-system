using Lms.Domain.Identity;

namespace Lms.Application.Features.Users.Dtos
{
    public sealed record LibrarianSummaryDto
    {
        public Guid LibrarianId { get; init; }
        public string Email { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Address { get; init; } = null!;
        public string LibraryCardNumber { get; init; } = null!;
        public UserStatus Status { get; init; }
        public int ManagedCategories { get; init; }
    }
}
