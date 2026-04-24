using FluentValidation;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Categories.Queries.GetCategoriesByBookId
{
    public sealed class GetCategoriesByBookIdQueryValidator : AbstractValidator<GetCategoriesByBookIdQuery>
    {
        public GetCategoriesByBookIdQueryValidator()
        {
            RuleFor(query => query.BookId)
                .NotEmpty()
                .WithErrorCode(BookErrors.IdRequired.Code)
                .WithMessage(BookErrors.IdRequired.Description);
        }
    }
}
