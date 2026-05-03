using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;

namespace Lms.Application.Features.Users.Queries.GetLibrarianById
{
    public sealed record GetLibrarianByIdQuery(Guid LibrarianId) : ICachedQuery<Result<LibrarianDto>>
    {
        public string CacheKey => $"users:librarian:{LibrarianId}";

        public string[] Tags => ["user", "librarian"];

        public TimeSpan Expiration => TimeSpan.FromHours(24);
    }
}
