using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Users.Queries.GetAdmins
{
    public sealed class GetAdminsQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetAdminsQuery, Result<PaginatedList<AdminDto>>>
    {
        public async Task<Result<PaginatedList<AdminDto>>> Handle(GetAdminsQuery request, CancellationToken cancellationToken)
        {
            var adminsQuery = db.Users
                .Where(user => user.Role == Role.Admin)
                .AsNoTracking();
            var totalCount = await adminsQuery.CountAsync(cancellationToken);
            var admins = await adminsQuery
                .OrderBy(admin => admin.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(admin => new AdminDto
                {
                    AdminId = admin.Id,
                    Email = admin.Email,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Address = admin.Address,
                    LibraryCardNumber = admin.LibraryCardNumber,
                    Status = admin.Status
                }).ToListAsync(cancellationToken);
            return new PaginatedList<AdminDto>
            {
                TotalCount = totalCount,
                PageSize = request.PageSize,
                PageNumber = request.Page,
                Items = admins,
                TotalPages = (int) Math.Ceiling((double) totalCount / request.PageSize)
            };
        }
    }
}
