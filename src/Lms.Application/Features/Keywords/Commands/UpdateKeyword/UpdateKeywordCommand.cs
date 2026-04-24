using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Keywords.Commands.UpdateKeyword
{
    public sealed record UpdateKeywordCommand(Guid KeywordId, string Name) : IRequest<Result<Updated>>;
}
