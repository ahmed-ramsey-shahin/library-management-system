using FluentValidation;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Audiences.Commands.DeleteAudience
{
    public sealed class DeleteAudienceCommandValidator : AbstractValidator<DeleteAudienceCommand>
    {
        public DeleteAudienceCommandValidator()
        {
            RuleFor(command => command.AudienceId)
                .NotEmpty()
                .WithErrorCode(AudienceErrors.IdRequired.Code)
                .WithMessage(AudienceErrors.IdRequired.Description);
        }
    }
}
