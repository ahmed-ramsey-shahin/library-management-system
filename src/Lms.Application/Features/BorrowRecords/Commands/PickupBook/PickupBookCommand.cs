using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.BorrowRecords.Commands.PickupBook
{
    public sealed record PickupBookCommand(Guid BorrowRecordId) : IRequest<Result<Updated>>;
}
