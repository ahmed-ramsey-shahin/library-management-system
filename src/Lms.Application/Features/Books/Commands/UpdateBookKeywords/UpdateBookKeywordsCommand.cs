using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateBookKeywords
{
    public sealed record UpdateBookKeywordsCommand(Guid BookId, List<Guid> KeywordIds) : IRequest<Result<Updated>>;
}
