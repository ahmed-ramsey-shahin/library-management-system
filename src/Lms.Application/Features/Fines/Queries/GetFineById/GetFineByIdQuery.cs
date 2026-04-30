using Lms.Application.Features.Fines.Dtos;
using Lms.Domain.Common.Results;
using MediatR;

namespace Lms.Application.Features.Fines.Queries.GetFineById
{
    public sealed record GetFineByIdQuery(Guid FineId) : IRequest<Result<FineDto>>;
}
