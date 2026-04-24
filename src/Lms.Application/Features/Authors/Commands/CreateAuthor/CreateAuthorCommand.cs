using Lms.Application.Features.Authors.Dtos;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Authors.Commands.CreateAuthor
{
    public sealed record CreateAuthorCommand(string Name) : IRequest<Result<AuthorDto>>;
}
