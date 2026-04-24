using FluentValidation;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Authors.Commands.DeleteAuthor
{
    public sealed class DeleteAuthorCommandValidator : AbstractValidator<DeleteAuthorCommand>
    {
        public DeleteAuthorCommandValidator()
        {
            RuleFor(command => command.AuthorId)
                .NotEmpty()
                .WithErrorCode(AuthorErrors.IdRequired.Code)
                .WithMessage(AuthorErrors.IdRequired.Description);
        }
    }
}
