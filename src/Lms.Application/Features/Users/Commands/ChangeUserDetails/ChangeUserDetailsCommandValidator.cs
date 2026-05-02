using FluentValidation;
using Lms.Application.Common;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Users.Commands.ChangeUserDetails
{
    public class ChangeUserDetailsCommandValidator : AbstractValidator<ChangeUserDetailsCommand>
    {
        public ChangeUserDetailsCommandValidator()
        {
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
