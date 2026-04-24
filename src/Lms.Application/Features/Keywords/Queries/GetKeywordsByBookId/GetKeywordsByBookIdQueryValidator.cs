using FluentValidation;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Keywords.Queries.GetKeywordsByBookId
{
    public sealed class GetKeywordsByBookIdQueryValidator : AbstractValidator<GetKeywordsByBookIdQuery>
    {
        public GetKeywordsByBookIdQueryValidator()
        {
            RuleFor(query => query.BookId)
                .NotEmpty()
                .WithErrorCode(BookErrors.IdRequired.Code)
                .WithMessage(BookErrors.IdRequired.Description);
        }
    }
}
