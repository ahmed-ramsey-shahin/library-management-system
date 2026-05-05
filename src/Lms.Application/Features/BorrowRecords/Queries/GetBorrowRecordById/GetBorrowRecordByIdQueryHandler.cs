using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Application.Features.Fines.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.BorrowRecords.Queries.GetBorrowRecordById
{
    public sealed class GetBorrowRecordByIdQueryHandler(
        IAppDbContext db,
        ILogger<GetBorrowRecordByIdQueryHandler> logger
    ) : IRequestHandler<GetBorrowRecordByIdQuery, Result<BorrowRecordDto>>
    {
        public async Task<Result<BorrowRecordDto>> Handle(
            GetBorrowRecordByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var borrowRecord = await db.BorrowRecords
                .AsNoTracking()
                .Where(record => record.Id == request.BorrowRecordId)
                .Select(record => new BorrowRecordDto
                {
                    BorrowRecordId = record.Id,
                    MemberId = record.MemberId,
                    BookCopyId = record.BookCopyId,
                    BookId = record.BookCopy.BookId,
                    Status = record.Status,
                    DueDate = record.DueDate,
                    PickupDeadline = record.PickupDeadline,
                    BorrowingCost = record.BorrowingCost,
                    RenewalCount = record.RenewalCount,
                    BookTitle = record.BookCopy.Book.Title,
                    Email = record.Member.Email,
                    FullName = $"{record.Member.FirstName} {record.Member.LastName}",
                    LibraryCardNumber = record.Member.LibraryCardNumber,
                    Fines = record.Fines.Select(fine => new FineDto
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
                    }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);

            if (borrowRecord is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("Borrow record retrieval aborted. No borrow record was found with Id {BorrowRecordId}.", request.BorrowRecordId);
                }

                return ApplicationErrors.BorrowRecordNotFound;
            }

            return borrowRecord;
        }
    }
}
