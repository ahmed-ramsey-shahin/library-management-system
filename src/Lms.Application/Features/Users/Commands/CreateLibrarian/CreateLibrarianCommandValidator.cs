using FluentValidation;
using Lms.Application.Common;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Users.Commands.CreateLibrarian
{
    public class CreateLibrarianCommandValidator : AbstractValidator<CreateLibrarianCommand>
    {
        public CreateLibrarianCommandValidator()
        {
            RuleFor(command => command.Email)
                .Matches("""(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])""")
                .WithErrorCode(ApplicationErrors.EmailInvalid.Code)
                .WithMessage(ApplicationErrors.EmailInvalid.Description);

            RuleFor(command => command.FirstName)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.NameLength.Code)
                .WithMessage(ApplicationErrors.NameLength.Description);

            RuleFor(command => command.LastName)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.NameLength.Code)
                .WithMessage(ApplicationErrors.NameLength.Description);

            RuleFor(command => command.PhoneNumber)
                .IsValidPhoneNumber()
                .WithErrorCode(ApplicationErrors.PhoneNumberInvalid.Code)
                .WithMessage(ApplicationErrors.PhoneNumberInvalid.Description);

            RuleFor(command => command.Address)
                .MaximumLength(512)
                .WithErrorCode(ApplicationErrors.AddressLength.Code)
                .WithMessage(ApplicationErrors.AddressLength.Description);
        }
    }
}
