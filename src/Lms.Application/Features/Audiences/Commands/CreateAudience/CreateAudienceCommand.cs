using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Audiences.Dtos;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Audiences.Commands.CreateAudience
{
    public sealed record CreateAudienceCommand(string Name, Guid IdempotencyKey) : IRequest<Result<AudienceDto>>, IIdempotentCommand;
}
