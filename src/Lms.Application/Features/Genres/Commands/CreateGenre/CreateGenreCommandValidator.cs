using FluentValidation;
using Lms.Application.Common.Errors;

namespace Lms.Application.Features.Genres.Commands.CreateGenre
{
    public sealed class CreateGenreCommandValidator : AbstractValidator<CreateGenreCommand>
    {
        public CreateGenreCommandValidator()
        {
            RuleFor(command => command.Name)
                .MaximumLength(50)
                .WithErrorCode(ApplicationErrors.GenreNameLength.Code)
                .WithMessage(ApplicationErrors.GenreNameLength.Description);
        }
    }
}
