using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Themes.Commands.UpdateTheme
{
    public sealed record UpdateThemeCommand(Guid ThemeId, string Name) : IRequest<Result<Updated>>;
}
