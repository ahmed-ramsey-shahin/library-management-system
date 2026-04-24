using FluentValidation;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Themes.Queries.GetThemesByBookId
{
    public sealed class GetThemesByBookIdQueryValidator : AbstractValidator<GetThemesByBookIdQuery>
    {
        public GetThemesByBookIdQueryValidator()
        {
            RuleFor(query => query.BookId)
                .NotEmpty()
                .WithErrorCode(BookErrors.IdRequired.Code)
                .WithMessage(BookErrors.IdRequired.Description);
        }
    }
}
