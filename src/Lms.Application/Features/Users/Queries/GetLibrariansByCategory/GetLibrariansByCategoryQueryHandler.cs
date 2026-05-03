using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Users.Queries.GetLibrariansByCategory
{
    public sealed class GetLibrariansByCategoryQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetLibrariansByCategoryQuery, Result<PaginatedList<LibrarianSummaryDto>>>
    {
        public async Task<Result<PaginatedList<LibrarianSummaryDto>>> Handle(GetLibrariansByCategoryQuery request, CancellationToken cancellationToken)
        {
            var librariansQuery = db.Users
                .AsNoTracking()
                .Where(user => Role.Librarian == user.Role && user.LibrarianCategories.Any(librarianCategory => librarianCategory.CategoryId == request.CategoryId));
            var totalCount = await librariansQuery.CountAsync(cancellationToken);
            var librarians = await librariansQuery
                .OrderBy(librarian => librarian.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(librarian => new LibrarianSummaryDto
                {
                    LibrarianId = librarian.Id,
                    Email = librarian.Email,
                    FirstName = librarian.FirstName,
                    LastName = librarian.LastName,
                    Address = librarian.Address,
                    LibraryCardNumber = librarian.LibraryCardNumber,
                    Status = librarian.Status,
                    ManagedCategories = librarian.LibrarianCategories.Count
                }).ToListAsync(cancellationToken);

            return new PaginatedList<LibrarianSummaryDto>
            {
                TotalCount = totalCount,
                PageSize = request.PageSize,
                PageNumber = request.Page,
                Items = librarians,
                TotalPages = (int) Math.Ceiling((double) totalCount / request.PageSize)
            };
        }
    }
}
