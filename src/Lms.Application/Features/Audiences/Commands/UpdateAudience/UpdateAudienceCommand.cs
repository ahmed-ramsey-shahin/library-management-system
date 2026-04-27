using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Audiences.Commands.UpdateAudience
{
    public sealed record UpdateAudienceCommand(Guid AudienceId, string Name) : IRequest<Result<Updated>>;
}
