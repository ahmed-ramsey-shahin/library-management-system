using FluentValidation;
using Lms.Application.Common.Errors;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Keywords.Commands.UpdateKeyword
{
    public sealed class UpdateKeywordCommandValidator : AbstractValidator<UpdateKeywordCommand>
    {
        public UpdateKeywordCommandValidator()
        {
            RuleFor(command => command.KeywordId)
                .NotEmpty()
                .WithErrorCode(KeywordErrors.IdRequired.Code)
                .WithMessage(KeywordErrors.IdRequired.Description);

            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.KeywordNameLength.Code)
                .WithMessage(ApplicationErrors.KeywordNameLength.Description);
        }
    }
}
