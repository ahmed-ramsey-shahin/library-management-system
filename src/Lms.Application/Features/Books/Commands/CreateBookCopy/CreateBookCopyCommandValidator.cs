using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Books.Commands.CreateBookCopy
{
    public sealed class CreateBookCopyCommandValidator : AbstractValidator<CreateBookCopyCommand>
    {
        public CreateBookCopyCommandValidator()
        {
            RuleFor(command => command.Barcode)
                .Matches(@"^CPY-\d{8}$")
                .WithErrorCode(ApplicationErrors.BarcodeInvalid.Code)
                .WithMessage(ApplicationErrors.BarcodeInvalid.Description);

            RuleFor(command => command.Location)
                .MaximumLength(100)
                .WithErrorCode(ApplicationErrors.LocationLength.Code)
                .WithMessage(ApplicationErrors.LocationLength.Description);
        }
    }
}
