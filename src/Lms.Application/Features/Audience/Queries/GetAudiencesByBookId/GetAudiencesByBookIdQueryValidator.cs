using FluentValidation;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Audiences.Queries.GetAudiencesByBookId
{
    public sealed class GetAudiencesByBookIdQueryValidator : AbstractValidator<GetAudiencesByBookIdQuery>
    {
        public GetAudiencesByBookIdQueryValidator()
        {
            RuleFor(query => query.BookId)
                .NotEmpty()
                .WithErrorCode(BookErrors.IdRequired.Code)
                .WithMessage(BookErrors.IdRequired.Description);
        }
    }
}
