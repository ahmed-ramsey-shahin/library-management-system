using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateBookThemes
{
    public sealed record UpdateBookThemesCommand(Guid BookId, List<Guid> ThemeIds) : IRequest<Result<Updated>>;
}
