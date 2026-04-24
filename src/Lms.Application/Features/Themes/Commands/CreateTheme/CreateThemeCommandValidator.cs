using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Themes.Commands.CreateTheme
{
    public sealed class CreateThemeCommandValidator : AbstractValidator<CreateThemeCommand>
    {
        public CreateThemeCommandValidator()
        {
            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.ThemeNameLength.Code)
                .WithMessage(ApplicationErrors.ThemeNameLength.Description);
        }
    }
}
