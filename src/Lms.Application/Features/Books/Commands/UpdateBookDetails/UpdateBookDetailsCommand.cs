using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateBookDetails
{
    public sealed record UpdateBookDetailsCommand(
        Guid BookId,
        string? Isbn,
        string? Issn,
        string? Title,
        string? Description,
        int? PageCount,
        Guid? PublisherId,
        DateOnly? PublishingDate,
        string? Edition,
        string? Language
    ) : IRequest<Result<Updated>>;
}
