using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Fines.Commands.DeleteFine
{
    public sealed record DeleteFineCommand(Guid BorrowRecordId, Guid FineId) : IRequest<Result<Deleted>>;
}
