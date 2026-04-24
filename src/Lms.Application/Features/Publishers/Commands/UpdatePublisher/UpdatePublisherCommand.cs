using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Publishers.Commands.UpdatePublisher
{
    public sealed record UpdatePublisherCommand(Guid PublisherId, string Name) : IRequest<Result<Updated>>;
}
