using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Themes.Dtos;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Themes.Commands.CreateTheme
{
    public sealed record CreateThemeCommand(string Name, Guid IdempotencyKey) : IRequest<Result<ThemeDto>>, IIdempotentCommand;
}
