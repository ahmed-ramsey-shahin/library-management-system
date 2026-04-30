using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Fines.Commands.IssueFine
{
    public class IssueFineCommandValidator : AbstractValidator<IssueFineCommand>
    {
        public IssueFineCommandValidator()
        {
            RuleFor(command => command.Description)
                .MaximumLength(500)
                .WithErrorCode(ApplicationErrors.FineDescriptionLength.Code)
                .WithMessage(ApplicationErrors.FineDescriptionLength.Description);
        }
    }
}
