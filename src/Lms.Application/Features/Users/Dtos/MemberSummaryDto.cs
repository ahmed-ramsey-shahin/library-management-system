using Lms.Domain.Identity;

namespace Lms.Application.Features.Users.Dtos
{
    public sealed record class MemberSummaryDto
    {
        public Guid MemberId { get; init; }
        public string Email { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Address { get; init; } = null!;
        public string LibraryCardNumber { get; init; } = null!;
        public UserStatus Status { get; init; }
        public bool HasUnpaidFines { get; init; }
        public bool HasLateBorrows { get; init; }
    }
}
