using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Fines.Commands.IssueFine;
using Lms.Application.Features.Fines.Dtos;
using Lms.Application.Features.Fines.Queries.GetFineById;
using Lms.Application.Features.Fines.Queries.GetFinesByBorrowRecordId;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/fines")]
    public class FinesController(ISender sender) : ApiController
    {
        [HttpPost]
        [Authorize(Roles = nameof(Role.Admin))]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("IssueFine")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> IssueFine(
            [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey,
            IssueFineRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new IssueFineCommand(request.BorrowRecordId, request.Amount, request.Description, DateTimeOffset.UtcNow, idempotencyKey), cancellationToken);
            return result.Match(id => CreatedAtAction(
                nameof(GetFineById),
                new
                {
                    id,
                },
                id
            ), Problem);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(FineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetFineById")]
        public async Task<IActionResult> GetFineById(
            [FromServices] IUser user,
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetFineByIdQuery(id, user.Id!.Value, user.UserRole!.Value), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/borrow-records/{borrowRecordId:guid}/fines")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedList<FineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetFinesByBorrowRecord")]
        public async Task<IActionResult> GetFinesByBorrowRecord(
            [FromServices] IUser user,
            [FromQuery] int pageSize,
            [FromQuery] int pageNumber,
            Guid borrowRecordId,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetFinesByBorrowRecordIdQuery(
                borrowRecordId,
                user.Id!.Value,
                user.UserRole!.Value,
                pageNumber,
                pageSize
            ), cancellationToken);
            return result.Match(Ok, Problem);
        }
    }
}
