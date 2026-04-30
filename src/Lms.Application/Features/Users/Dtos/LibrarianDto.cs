using Lms.Application.Features.Categories.Dtos;
using Lms.Domain.Identity;

namespace Lms.Application.Features.Users.Dtos
{
    public sealed record LibrarianDto
    {
        public Guid LibrarianId { get; init; }
        public string Email { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Address { get; init; } = null!;
        public string LibraryCardNumber { get; init; } = null!;
        public UserStatus Status { get; init; }
        public List<CategoryDto> Categories { get; init; } = null!;
    }
}
