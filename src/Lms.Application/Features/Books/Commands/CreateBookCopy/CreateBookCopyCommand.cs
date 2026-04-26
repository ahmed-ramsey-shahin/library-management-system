using Lms.Application.Common.Interfaces;
using Lms.Domain.Catalog;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.CreateBookCopy
{
    public sealed record CreateBookCopyCommand(
        Guid BookId,
        string Barcode,
        BookCopyStatus? Status,
        BookCopyState? State,
        string Location,
        DateOnly? AcquisitionDate,
        Guid IdempotencyKey
    ) : IRequest<Result<Guid>>, IIdempotentCommand;
}
