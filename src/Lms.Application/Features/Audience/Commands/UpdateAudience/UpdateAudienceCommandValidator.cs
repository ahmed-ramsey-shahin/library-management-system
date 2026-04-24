using FluentValidation;
using Lms.Application.Common.Errors;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Audiences.Commands.UpdateAudience
{
    public sealed class UpdateAudienceCommandValidator : AbstractValidator<UpdateAudienceCommand>
    {
        public UpdateAudienceCommandValidator()
        {
            RuleFor(command => command.AudienceId)
                .NotEmpty()
                .WithErrorCode(AudienceErrors.IdRequired.Code)
                .WithMessage(AudienceErrors.IdRequired.Description);

            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.AudienceNameLength.Code)
                .WithMessage(ApplicationErrors.AudienceNameLength.Description);
        }
    }
}
