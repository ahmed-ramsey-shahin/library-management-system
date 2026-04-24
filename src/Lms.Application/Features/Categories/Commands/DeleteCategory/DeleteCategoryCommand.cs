using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Categories.Commands.DeleteCategory
{
    public sealed record DeleteCategoryCommand(Guid CategoryId) : IRequest<Result<Deleted>>;
}
