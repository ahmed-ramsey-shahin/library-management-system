using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Authors.Commands.DeleteAuthor
{
    public sealed record DeleteAuthorCommand(Guid AuthorId) : IRequest<Result<Deleted>>;
}
