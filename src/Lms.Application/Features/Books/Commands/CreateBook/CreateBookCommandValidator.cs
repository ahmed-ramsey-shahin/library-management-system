using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Books.Commands.CreateBook
{
    public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
            RuleFor(command => command.Isbn)
                .Matches("^(?:ISBN(?:-1[03])?:? )?(?=[-0-9 ]{17}$|[-0-9X ]{13}$|[0-9X]{10}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$")
                .WithErrorCode(ApplicationErrors.BookIsbnInvalid.Code)
                .WithMessage(ApplicationErrors.BookIsbnInvalid.Description);

            RuleFor(command => command.Issn)
                .Matches(@"^\d{4}-\d{3}[\dX]$")
                .WithErrorCode(ApplicationErrors.BookIsbnInvalid.Code)
                .WithMessage(ApplicationErrors.BookIsbnInvalid.Description);

            RuleFor(command => command.Title)
                .MaximumLength(255)
                .WithErrorCode(ApplicationErrors.BookTitleLength.Code)
                .WithMessage(ApplicationErrors.BookTitleLength.Description);

            RuleFor(command => command.Description)
                .MaximumLength(1024)
                .WithErrorCode(ApplicationErrors.BookDescriptionLength.Code)
                .WithMessage(ApplicationErrors.BookDescriptionLength.Description);
        }
    }
}
