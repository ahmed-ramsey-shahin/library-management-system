using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.BorrowRecords.Dto;
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
            // TODO: Complete the borrow record dto after making the fine and user commands.
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
                    RenewalCount = record.RenewalCount
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
