using FluentValidation;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Keywords.Commands.DeleteKeyword
{
    public sealed class DeleteKeywordCommandValidator : AbstractValidator<DeleteKeywordCommand>
    {
        public DeleteKeywordCommandValidator()
        {
            RuleFor(command => command.KeywordId)
                .NotEmpty()
                .WithErrorCode(KeywordErrors.IdRequired.Code)
                .WithMessage(KeywordErrors.IdRequired.Description);
        }
    }
}
