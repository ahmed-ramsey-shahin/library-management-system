using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.MarkBookCopyAsMaintenance
{
    public sealed record MarkBookCopyAsMaintenanceCommand(
        Guid BookId,
        Guid CopyId,
        byte[] Version
    ) : IRequest<Result<Updated>>;
}
