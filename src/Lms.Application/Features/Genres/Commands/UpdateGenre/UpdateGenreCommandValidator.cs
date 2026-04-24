using FluentValidation;
using Lms.Application.Common.Errors;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Genres.Commands.UpdateGenre
{
    public class UpdateGenreCommandValidator : AbstractValidator<UpdateGenreCommand>
    {
        public UpdateGenreCommandValidator()
        {
            RuleFor(command => command.GenreId)
                .NotEmpty()
                .WithErrorCode(GenreErrors.IdRequired.Code)
                .WithMessage(GenreErrors.IdRequired.Description);

            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.GenreNameLength.Code)
                .WithMessage(ApplicationErrors.GenreNameLength.Description);
        }
    }
}
