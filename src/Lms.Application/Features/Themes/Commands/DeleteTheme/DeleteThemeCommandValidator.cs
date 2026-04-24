using FluentValidation;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Themes.Commands.DeleteTheme
{
    public sealed class DeleteThemeCommandValidator : AbstractValidator<DeleteThemeCommand>
    {
        public DeleteThemeCommandValidator()
        {
            RuleFor(command => command.ThemeId)
                .NotEmpty()
                .WithErrorCode(ThemeErrors.IdRequired.Code)
                .WithMessage(ThemeErrors.IdRequired.Description);
        }
    }
}
