using FluentValidation;
using Lms.Domain.Catalog;

namespace Lms.Application.Features.Genres.Queries.GetGenresByBookId
{
    public sealed class GetGenresByBookIdQueryValidator : AbstractValidator<GetGenresByBookIdQuery>
    {
        public GetGenresByBookIdQueryValidator()
        {
            RuleFor(query => query.BookId)
                .NotEmpty()
                .WithErrorCode(BookErrors.IdRequired.Code)
                .WithMessage(BookErrors.IdRequired.Description);
        }
    }
}
