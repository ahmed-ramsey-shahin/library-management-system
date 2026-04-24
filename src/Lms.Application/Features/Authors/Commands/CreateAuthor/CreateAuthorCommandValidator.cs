using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Authors.Commands.CreateAuthor
{
    public sealed class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
    {
        public CreateAuthorCommandValidator()
        {
            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.AuthorNameLength.Code)
                .WithMessage(ApplicationErrors.AuthorNameLength.Description);
        }
    }
}
