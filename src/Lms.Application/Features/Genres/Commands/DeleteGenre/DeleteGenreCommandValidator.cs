using FluentValidation;
using Lms.Domain.Metadata;

namespace Lms.Application.Features.Genres.Commands.DeleteGenre
{
    public sealed class DeleteGenreCommandValidator : AbstractValidator<DeleteGenreCommand>
    {
        public DeleteGenreCommandValidator()
        {
            RuleFor(command => command.GenreId)
                .NotEmpty()
                .WithErrorCode(GenreErrors.IdRequired.Code)
                .WithMessage(GenreErrors.IdRequired.Description);
        }
    }
}
