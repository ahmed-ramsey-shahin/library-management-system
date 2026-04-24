using FluentValidation;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Publishers.Commands.DeletePublisher
{
    public sealed class DeletePublisherCommandValidator : AbstractValidator<DeletePublisherCommand>
    {
        public DeletePublisherCommandValidator()
        {
            RuleFor(command => command.PublisherId)
                .NotEmpty()
                .WithErrorCode(PublisherErrors.IdRequired.Code)
                .WithMessage(PublisherErrors.IdRequired.Description);
        }
    }
}
