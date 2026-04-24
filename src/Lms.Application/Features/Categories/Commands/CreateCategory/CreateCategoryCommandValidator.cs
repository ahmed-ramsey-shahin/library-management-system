using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Categories.Commands.CreateCategory
{
    public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.CategoryNameLength.Code)
                .WithMessage(ApplicationErrors.CategoryNameLength.Description);
        }
    }
}
