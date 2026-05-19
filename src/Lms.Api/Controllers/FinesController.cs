using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.Fines.Commands.ChangeFineAmount;
using Lms.Application.Features.Fines.Commands.DeleteFine;
using Lms.Application.Features.Fines.Commands.IssueFine;
using Lms.Application.Features.Fines.Commands.PayFine;
using Lms.Application.Features.Fines.Dtos;
using Lms.Application.Features.Fines.Queries.GetFineById;
using Lms.Application.Features.Fines.Queries.GetFinesByBorrowRecordId;
using Lms.Application.Features.Fines.Queries.GetMemberFines;
using Lms.Application.Features.Fines.Queries.GetUnpaidFinesByCategory;
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

        [HttpGet("/api/v{version:apiVersion}/users/members/{memberId:guid}/fines")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(typeof(PaginatedList<FineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetMemberFines")]
        public async Task<IActionResult> GetMemberFines(
            [FromQuery] int pageSize,
            [FromQuery] int pageNumber,
            Guid memberId,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetMemberFinesQuery(
                memberId,
                pageNumber,
                pageSize
            ), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/users/members/me/fines")]
        [Authorize(Roles = nameof(Role.Member))]
        [ProducesResponseType(typeof(PaginatedList<FineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetMyFines")]
        public async Task<IActionResult> GetMyFines(
            [FromQuery] int pageSize,
            [FromQuery] int pageNumber,
            [FromServices] IUser user,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetMemberFinesQuery(
                user.Id!.Value,
                pageNumber,
                pageSize
            ), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/categories/{categoryId:guid}/fines/unpaid")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(typeof(PaginatedList<FineDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetUnpaidFinesByCategory")]
        public async Task<IActionResult> GetUnpaidFinesByCategory(
            [FromQuery] int pageSize,
            [FromQuery] int pageNumber,
            Guid categoryId,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetUnpaidFinesByCategoryQuery(
                categoryId,
                pageSize,
                pageNumber
            ), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpPut("/api/v{version:apiVersion}/borrow-records/{borrowRecordId:guid}/fines/{fineId:guid}/amount")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        [EndpointName("ChangeFineAmount")]
        public async Task<IActionResult> ChangeFineAmount(
            Guid borrowRecordId,
            Guid fineId,
            [FromBody] ChangeFineAmountRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new ChangeFineAmountCommand(borrowRecordId, fineId, request.NewAmount), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpDelete("/api/v{version:apiVersion}/borrow-records/{borrowRecordId:guid}/fines/{fineId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        [EndpointName("DeleteFine")]
        public async Task<IActionResult> DeleteFine(
            Guid borrowRecordId,
            Guid fineId,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new DeleteFineCommand(borrowRecordId, fineId), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPost("/api/v{version:apiVersion}/borrow-records/{borrowRecordId:guid}/fines/{fineId:guid}/payments")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        [EndpointName("PayFine")]
        public async Task<IActionResult> PayFine(
            Guid borrowRecordId,
            Guid fineId,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new PayFineCommand(borrowRecordId, fineId), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }
    }
}
