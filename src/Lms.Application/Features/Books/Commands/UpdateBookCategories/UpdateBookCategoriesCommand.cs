using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateBookCategories
{
    public sealed record UpdateBookCategoriesCommand(Guid BookId, List<Guid> CategoryIds) : IRequest<Result<Updated>>;
}
