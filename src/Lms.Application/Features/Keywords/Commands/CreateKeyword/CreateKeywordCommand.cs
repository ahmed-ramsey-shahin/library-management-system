using Lms.Application.Features.Keywords.Dtos;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Keywords.Commands.CreateKeyword
{
    public sealed record CreateKeywordCommand(string Name) : IRequest<Result<KeywordDto>>;
}
