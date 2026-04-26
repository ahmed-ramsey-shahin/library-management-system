using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Books.Commands.UpdateBookAudiences
{
    public sealed record UpdateBookAudiencesCommand(Guid BookId, List<Guid> AudienceIds) : IRequest<Result<Updated>>;
}
