using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.ReturnBook
{
    public sealed record ReturnBookCommand(
        Guid BorrowRecordId
    ) : IRequest<Result<Updated>>;
}
