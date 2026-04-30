using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Publishers.Dtos;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Publishers.Commands.CreatePublisher
{
    public sealed record CreatePublisherCommand(string Name, Guid IdempotencyKey) : IRequest<Result<PublisherDto>>, IIdempotentCommand;
}
