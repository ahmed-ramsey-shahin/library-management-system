using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Audiences.Commands.CreateAudience
{
    public sealed class CreateAudienceCommandValidator : AbstractValidator<CreateAudienceCommand>
    {
        public CreateAudienceCommandValidator()
        {
            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.AudienceNameLength.Code)
                .WithMessage(ApplicationErrors.AudienceNameLength.Description);
        }
    }
}
