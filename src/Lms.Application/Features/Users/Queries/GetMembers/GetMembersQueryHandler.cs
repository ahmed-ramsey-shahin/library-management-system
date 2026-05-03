using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Users.Queries.GetMembers
{
    public sealed class GetMembersQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetMembersQuery, Result<PaginatedList<MemberSummaryDto>>>
    {
        public async Task<Result<PaginatedList<MemberSummaryDto>>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            var membersQuery = db.Users
                .Where(user => user.Role == Role.Member)
                .AsNoTracking();
            var totalCount = await membersQuery.CountAsync(cancellationToken);
            var members = await membersQuery
                .OrderBy(member => member.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(member => new MemberSummaryDto
                {
                    MemberId = member.Id,
                    Email = member.Email,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Address = member.Address,
                    LibraryCardNumber = member.LibraryCardNumber,
                    Status = member.Status,
                    HasLateBorrows = member.BorrowRecords.Any(record => record.Status == BorrowRecordStatus.Late),
                    HasUnpaidFines = member.Fines.Any(fine => fine.Status == FineStatus.Unpaid)
                }).ToListAsync(cancellationToken);
            return new PaginatedList<MemberSummaryDto>
            {
                TotalCount = totalCount,
                PageSize = request.PageSize,
                PageNumber = request.Page,
                Items = members,
                TotalPages = (int) Math.Ceiling((double) totalCount / request.PageSize)
            };
        }
    }
}
