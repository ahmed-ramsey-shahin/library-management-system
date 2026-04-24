using FluentValidation;
using Lms.Application.Common.Errors;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Themes.Commands.UpdateTheme
{
    public sealed class UpdateThemeCommandValidator : AbstractValidator<UpdateThemeCommand>
    {
        public UpdateThemeCommandValidator()
        {
            RuleFor(command => command.ThemeId)
                .NotEmpty()
                .WithErrorCode(ThemeErrors.IdRequired.Code)
                .WithMessage(ThemeErrors.IdRequired.Description);

            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.ThemeNameLength.Code)
                .WithMessage(ApplicationErrors.ThemeNameLength.Description);
        }
    }
}
