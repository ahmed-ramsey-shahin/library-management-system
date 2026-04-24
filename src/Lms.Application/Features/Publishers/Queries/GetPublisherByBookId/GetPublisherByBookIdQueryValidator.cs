using FluentValidation;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Publishers.Queries.GetPublisherByBookId
{
    public sealed class GetPublisherByBookIdQueryValidator : AbstractValidator<GetPublisherByBookIdQuery>
    {
        public GetPublisherByBookIdQueryValidator()
        {
            RuleFor(query => query.BookId)
                .NotEmpty()
                .WithErrorCode(BookErrors.IdRequired.Code)
                .WithMessage(BookErrors.IdRequired.Description);
        }
    }
}
