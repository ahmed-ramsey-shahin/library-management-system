using Lms.Application.Common.Errors;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Books.Dtos;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Application.Features.Fines.Dtos;
using Lms.Application.Features.Users.Dtos;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.Application.Features.Users.Queries.GetMemberById
{
    public sealed class GetMemberByIdQueryHandler(
        IAppDbContext db,
        ILogger<GetMemberByIdQueryHandler> logger
    ) : IRequestHandler<GetMemberByIdQuery, Result<MemberDto>>
    {
        public async Task<Result<MemberDto>> Handle(
            GetMemberByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var member = await db
                .Users.AsNoTracking()
                .Where(user => user.Role == Role.Member && user.Id == request.MemberId)
                .Select(member => new MemberDto
                {
                    MemberId = member.Id,
                    Email = member.Email,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Address = member.Address,
                    LibraryCardNumber = member.LibraryCardNumber,
                    Status = member.Status,
                    BorrowRecords = member.BorrowRecords.Select(record => new BorrowRecordSummaryDto
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
                    }).ToList(),
                    Fines = member.Fines.Select(fine => new FineDto
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
                    }).ToList(),
                    Books = member.BorrowRecords
                        .Select(record => record.BookCopy.Book)
                        .Distinct()
                        .Select(book => new BookSummaryDto
                        {
                            BookId = book.Id,
                            Isbn = book.Isbn,
                            Title = book.Title,
                            Edition = book.Edition,
                            AvailableCopies = book.BookCopies.Count(copy => copy.State == BookCopyState.Available)
                        }).ToList()
                }).AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

            if (member is null)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning(
                        "Could not return member {MemberId}. No member was found with ID {MemberId}.",
                        request.MemberId,
                        request.MemberId
                    );
                }

                return ApplicationErrors.UserNotFound;
            }

            return member;
        }
    }
}
