using FluentValidation;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Categories.Commands.DeleteCategory
{
    public sealed class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(command => command.CategoryId)
                .NotEmpty()
                .WithErrorCode(CategoryErrors.IdRequired.Code)
                .WithMessage(CategoryErrors.IdRequired.Description);
        }
    }
}
