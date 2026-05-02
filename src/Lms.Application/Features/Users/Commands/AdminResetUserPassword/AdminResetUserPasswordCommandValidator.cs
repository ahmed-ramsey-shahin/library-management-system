using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Users.Commands.AdminResetUserPassword
{
    public class AdminResetUserPasswordCommandValidator : AbstractValidator<AdminResetUserPasswordCommand>
    {
        public AdminResetUserPasswordCommandValidator()
        {
            RuleFor(command => command.Password)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
                .WithErrorCode(ApplicationErrors.PasswordInvalid.Code)
                .WithMessage(ApplicationErrors.PasswordInvalid.Description);
        }
    }
}
