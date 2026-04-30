using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Fines.Dtos;
using Lms.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Fines.Queries.GetFineById
{
    public sealed class GetFineByIdQueryHandler(
        IAppDbContext db,
        ILogger<GetFineByIdQueryHandler> logger
    ) : IRequestHandler<GetFineByIdQuery, Result<FineDto>>
    {
        public async Task<Result<FineDto>> Handle(GetFineByIdQuery request, CancellationToken cancellationToken)
        {
            var fine = await db.Fines
                .AsNoTracking()
                .Where(fine => fine.Id == request.FineId)
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
                }).FirstOrDefaultAsync(cancellationToken);

            if (fine is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning("No fine was found with ID {FineId}.", request.FineId);
                }

                return ApplicationErrors.FineNotFound;
            }

            return fine;
        }
    }
}
