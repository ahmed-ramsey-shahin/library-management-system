using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.BorrowRecords.Commands.BorrowBook;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Application.Features.BorrowRecords.Queries.GetBorrowRecordById;
using Lms.Application.Features.BorrowRecords.Queries.GetMemberActiveBorrowings;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/borrow-records")]
    public class BorrowRecordsController(ISender sender) : ApiController
    {
        [HttpPost]
        [Authorize(Roles = nameof(Role.Member))]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [MapToApiVersion("1.0")]
        [EndpointName("BorrowBook")]
        public async Task<IActionResult> BorrowBook(
            [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey,
            [FromBody] BorrowBookRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new BorrowBookCommand(
                request.BookId,
                request.DueDate,
                request.PickupDeadline,
                idempotencyKey
            ), cancellationToken);
            return result.Match(id => CreatedAtAction(
                nameof(GetBorrowRecord),
                new
                {
                    id,
                },
                id
            ), Problem);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(BorrowRecordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetBorrowRecord")]
        public async Task<IActionResult> GetBorrowRecord(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetBorrowRecordByIdQuery(id), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/users/members/{memberId:guid}/borrow-records/active")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(typeof(PaginatedList<BorrowRecordSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetMemberActiveBorrowings")]
        public async Task<IActionResult> GetMemberActiveBorrowings(
            Guid memberId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetMemberActiveBorrowingsQuery(memberId, pageSize, pageNumber), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/users/members/me/borrow-records/active")]
        [Authorize(Roles = nameof(Role.Member))]
        [ProducesResponseType(typeof(PaginatedList<BorrowRecordSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetMyActiveBorrowings")]
        public async Task<IActionResult> GetMyActiveBorrowings(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromServices] IUser currnetUser,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetMemberActiveBorrowingsQuery(currnetUser.Id!.Value, pageSize, pageNumber), cancellationToken);
            return result.Match(Ok, Problem);
        }
    }
}
