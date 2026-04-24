using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Keywords.Commands.CreateKeyword
{
    public sealed class CreateKeywordCommandValidator : AbstractValidator<CreateKeywordCommand>
    {
        public CreateKeywordCommandValidator()
        {
            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.KeywordNameLength.Code)
                .WithMessage(ApplicationErrors.KeywordNameLength.Description);
        }
    }
}
