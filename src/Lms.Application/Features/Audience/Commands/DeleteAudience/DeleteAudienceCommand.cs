using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Audiences.Commands.DeleteAudience
{
    public sealed record DeleteAudienceCommand(Guid AudienceId) : IRequest<Result<Deleted>>;
}
