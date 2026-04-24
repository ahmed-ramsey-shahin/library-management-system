using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Themes.Commands.DeleteTheme
{
    public sealed record DeleteThemeCommand(Guid ThemeId) : IRequest<Result<Deleted>>;
}
