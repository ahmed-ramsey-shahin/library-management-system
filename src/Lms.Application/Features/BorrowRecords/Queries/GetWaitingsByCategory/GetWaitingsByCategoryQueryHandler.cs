using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Domain.Circulation;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.BorrowRecords.Queries.GetWaitingsByCategory
{
    public class GetWaitingsByCategoryQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetWaitingsByCategoryQuery, Result<PaginatedList<BorrowRecordSummary>>>
    {
        public async Task<Result<PaginatedList<BorrowRecordSummary>>> Handle(GetWaitingsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var borrowRecordsQuery = db.BorrowRecords
                .AsNoTracking()
                .Where(record => record.Status == BorrowRecordStatus.Waiting && record.BookCopy.Book.BookCategories.Any(bookCategory => bookCategory.CategoryId == request.CategoryId));
            var totalCount = await borrowRecordsQuery.CountAsync(cancellationToken);

            if (totalCount <= 0)
            {
                return new PaginatedList<BorrowRecordSummary>
                {
                    PageSize = request.PageSize,
                    PageNumber = request.Page,
                    Items = [],
                    TotalCount = 0,
                    TotalPages = 0,
                };
            }

            var borrowRecords = await borrowRecordsQuery
                .OrderBy(record => record.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(record => new BorrowRecordSummary
                {
                    BorrowRecordId = record.Id,
                    MemberId = record.MemberId,
                    BookCopyId = record.BookCopyId,
                    BookId = record.BookCopy.BookId,
                    BookTitle = record.BookCopy.Book.Title,
                    BookCopyLocation = record.BookCopy.Location,
                    DueDate = record.DueDate,
                    BorrowingCost = record.BorrowingCost,
                    PickupDeadline = record.PickupDeadline
                }).ToListAsync(cancellationToken);

            return new PaginatedList<BorrowRecordSummary>
            {
                PageSize = request.PageSize,
                PageNumber = request.Page,
                Items = borrowRecords,
                TotalCount = totalCount,
                TotalPages = (int) Math.Ceiling((double) totalCount / request.PageSize),
            };
        }
    }
}
