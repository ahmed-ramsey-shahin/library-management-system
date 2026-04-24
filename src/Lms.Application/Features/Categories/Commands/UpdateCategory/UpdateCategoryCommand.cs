using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Categories.Commands.UpdateCategory
{
    public sealed record UpdateCategoryCommand(Guid CategoryId, string Name) : IRequest<Result<Updated>>;
}
