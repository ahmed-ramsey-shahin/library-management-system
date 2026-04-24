using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Publishers.Commands.DeletePublisher
{
    public sealed record DeletePublisherCommand(Guid PublisherId) : IRequest<Result<Deleted>>;
}
