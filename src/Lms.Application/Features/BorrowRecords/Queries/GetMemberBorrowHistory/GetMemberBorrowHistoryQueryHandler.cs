using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lms.Application.Features.BorrowRecords.Queries.GetMemberBorrowHistory
{
    public sealed class GetMemberBorrowHistoryQueryHandler(
        IAppDbContext db
    ) : IRequestHandler<GetMemberBorrowHistoryQuery, Result<PaginatedList<BorrowRecordSummaryDto>>>
    {
        public async Task<Result<PaginatedList<BorrowRecordSummaryDto>>> Handle(GetMemberBorrowHistoryQuery request, CancellationToken cancellationToken)
        {
            var borrowRecordsQuery = db.BorrowRecords
                .AsNoTracking()
                .Where(record => record.MemberId == request.MemberId);
            var totalCount = await borrowRecordsQuery.CountAsync(cancellationToken);

            if (totalCount <= 0)
            {
                return new PaginatedList<BorrowRecordSummaryDto>
                {
                    PageSize = request.PageSize,
                    PageNumber = request.Page,
                    Items = [],
                    TotalCount = 0,
                    TotalPages = 0
                };
            }

            var borrowRecords = await borrowRecordsQuery
                .OrderByDescending(record => record.CreatedAt)
                .ThenBy(record => record.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(record => new BorrowRecordSummaryDto
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

            return new PaginatedList<BorrowRecordSummaryDto>
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
