using Lms.Application.Features.Books.Dtos;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Application.Features.Fines.Dtos;
using Lms.Domain.Identity;

namespace Lms.Application.Features.Users.Dtos
{
    public sealed record MemberDto
    {
        public Guid MemberId { get; init; }
        public string Email { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Address { get; init; } = null!;
        public string LibraryCardNumber { get; init; } = null!;
        public UserStatus Status { get; init; }
        public List<BorrowRecordSummaryDto> BorrowRecords { get; init; } = null!;
        public List<FineDto> Fines { get; init; } = null!;
        public List<BookSummaryDto> Books { get; init; } = null!;
    }
}
