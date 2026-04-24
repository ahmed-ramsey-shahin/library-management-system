using Lms.Application.Features.Categories.Dtos;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Categories.Commands.CreateCategory
{
    public sealed record CreateCategoryCommand(string Name) : IRequest<Result<CategoryDto>>;
}
