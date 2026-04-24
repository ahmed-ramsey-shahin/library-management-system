using FluentValidation;
using Lms.Application.Common.Errors;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Categories.Commands.UpdateCategory
{
    public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(command => command.CategoryId)
                .NotEmpty()
                .WithErrorCode(CategoryErrors.IdRequired.Code)
                .WithMessage(CategoryErrors.IdRequired.Description);

            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.CategoryNameLength.Code)
                .WithMessage(ApplicationErrors.CategoryNameLength.Description);
        }
    }
}
