using FluentValidation;
using Lms.Application.Common.Errors;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Authors.Commands.UpdateAuthor
{
    public sealed class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
    {
        public UpdateAuthorCommandValidator()
        {
            RuleFor(command => command.AuthorId)
                .NotEmpty()
                .WithErrorCode(AuthorErrors.IdRequired.Code)
                .WithMessage(AuthorErrors.IdRequired.Description);

            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.AuthorNameLength.Code)
                .WithMessage(ApplicationErrors.AuthorNameLength.Description);
        }
    }
}
