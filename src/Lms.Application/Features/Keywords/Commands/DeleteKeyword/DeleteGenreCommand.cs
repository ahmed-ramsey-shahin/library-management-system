using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Keywords.Commands.DeleteKeyword
{
    public sealed record DeleteKeywordCommand(Guid KeywordId) : IRequest<Result<Deleted>>;
}
