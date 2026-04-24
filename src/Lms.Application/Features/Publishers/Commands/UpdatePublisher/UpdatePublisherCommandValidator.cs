using FluentValidation;
using Lms.Application.Common.Errors;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Publishers.Commands.UpdatePublisher
{
    public sealed class UpdatePublisherCommandValidator : AbstractValidator<UpdatePublisherCommand>
    {
        public UpdatePublisherCommandValidator()
        {
            RuleFor(command => command.PublisherId)
                .NotEmpty()
                .WithErrorCode(PublisherErrors.IdRequired.Code)
                .WithMessage(PublisherErrors.IdRequired.Description);

            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.PublisherNameLength.Code)
                .WithMessage(ApplicationErrors.PublisherNameLength.Description);
        }
    }
}
