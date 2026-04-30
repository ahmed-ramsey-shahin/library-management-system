using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Fines.Dtos;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.Fines.Queries.GetUnpaidFinesByCategory
{
    public sealed class GetUnpaidFinesByCategoryQueryHandler(IAppDbContext db) : IRequestHandler<GetUnpaidFinesByCategoryQuery, Result<PaginatedList<FineDto>>>
    {
        public async Task<Result<PaginatedList<FineDto>>> Handle(GetUnpaidFinesByCategoryQuery request, CancellationToken cancellationToken)
        {
            var finesQuery = db.Fines
                .AsNoTracking()
                .Where(fine => fine.Status == FineStatus.Unpaid && fine.BorrowRecord.BookCopy.Book.BookCategories.Any(bookCategory => bookCategory.CategoryId == request.CategoryId));
            var totalCount = await finesQuery.CountAsync(cancellationToken);

            if (totalCount <= 0)
            {
                return new PaginatedList<FineDto>
                {
                    Items = [],
                    PageNumber = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    TotalPages = 0
                };
            }

            var fines = await finesQuery
                .OrderBy(fine => fine.FineDate)
                .ThenByDescending(fine => fine.Amount)
                .ThenBy(fine => fine.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(fine => new FineDto
                {
                    FineId = fine.Id,
                    MemberId = fine.MemberId,
                    MemberName = $"{fine.Member.FirstName} {fine.Member.LastName}",
                    BorrowRecordId = fine.BorrowRecordId,
                    BookTitle = fine.BorrowRecord.BookCopy.Book.Title,
                    Status = fine.Status,
                    Amount = fine.Amount,
                    Description = fine.Description,
                    FineDate = fine.FineDate,
                    PaidAt = fine.PaidAt
                }).ToListAsync(cancellationToken);
            return new PaginatedList<FineDto>
            {
                TotalCount = totalCount,
                PageSize = request.PageSize,
                PageNumber = request.Page,
                Items = fines,
                TotalPages = (int) Math.Ceiling((double) totalCount / request.PageSize)
            };
        }
    }
}
