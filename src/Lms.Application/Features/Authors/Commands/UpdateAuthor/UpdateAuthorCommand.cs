using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Authors.Commands.UpdateAuthor
{
    public sealed record UpdateAuthorCommand(Guid AuthorId, string Name) : IRequest<Result<Updated>>;
}
