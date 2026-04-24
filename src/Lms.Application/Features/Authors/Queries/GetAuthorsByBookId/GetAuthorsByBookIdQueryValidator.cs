using FluentValidation;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Authors.Queries.GetAuthorsByBookId
{
    public sealed class GetAuthorsByBookIdQueryValidator : AbstractValidator<GetAuthorsByBookIdQuery>
    {
        public GetAuthorsByBookIdQueryValidator()
        {
            RuleFor(query => query.BookId)
                .NotEmpty()
                .WithErrorCode(BookErrors.IdRequired.Code)
                .WithMessage(BookErrors.IdRequired.Description);
        }
    }
}
