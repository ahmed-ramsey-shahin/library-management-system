using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Publishers.Commands.CreatePublisher
{
    public sealed class CreatePublisherCommandValidator : AbstractValidator<CreatePublisherCommand>
    {
        public CreatePublisherCommandValidator()
        {
            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.PublisherNameLength.Code)
                .WithMessage(ApplicationErrors.PublisherNameLength.Description);
        }
    }
}
