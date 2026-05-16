using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Common.Interfaces;
using Lms.Application.Common.Models;
using Lms.Application.Features.BorrowRecords.Commands.BorrowBook;
using Lms.Application.Features.BorrowRecords.Commands.CancelBorrowRecord;
using Lms.Application.Features.BorrowRecords.Commands.MarkRecordAsLate;
using Lms.Application.Features.BorrowRecords.Commands.OverrideDueDate;
using Lms.Application.Features.BorrowRecords.Commands.RejectBorrowRecord;
using Lms.Application.Features.BorrowRecords.Commands.RenewBook;
using Lms.Application.Features.BorrowRecords.Dto;
using Lms.Application.Features.BorrowRecords.Queries.GetBorrowRecordById;
using Lms.Application.Features.BorrowRecords.Queries.GetMemberActiveBorrowings;
using Lms.Application.Features.BorrowRecords.Queries.GetMemberBorrowHistory;
using Lms.Application.Features.BorrowRecords.Queries.GetMemberPendingRequests;
using Lms.Application.Features.BorrowRecords.Queries.GetOverdueBorrowings;
using Lms.Application.Features.BorrowRecords.Queries.GetReadyForPickup;
using Lms.Application.Features.BorrowRecords.Queries.GetWaitingsByCategory;
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

        [HttpGet("/api/v{version:apiVersion}/users/members/{memberId:guid}/borrow-records/history")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(typeof(PaginatedList<BorrowRecordSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetMemberBorrowHistory")]
        public async Task<IActionResult> GetMemberBorrowHistory(
            Guid memberId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetMemberBorrowHistoryQuery(memberId, pageSize, pageNumber), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/users/members/me/borrow-records/history")]
        [Authorize(Roles = nameof(Role.Member))]
        [ProducesResponseType(typeof(PaginatedList<BorrowRecordSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetMyBorrowHistory")]
        public async Task<IActionResult> GetMyBorrowHistory(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromServices] IUser currnetUser,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetMemberBorrowHistoryQuery(currnetUser.Id!.Value, pageSize, pageNumber), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/users/members/{memberId:guid}/borrow-records/pendings")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(typeof(PaginatedList<BorrowRecordSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetMemberPendingRequests")]
        public async Task<IActionResult> GetMemberPendingRequests(
            Guid memberId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetMemberPendingRequestsQuery(memberId, pageSize, pageNumber), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/users/members/me/borrow-records/pending")]
        [Authorize(Roles = nameof(Role.Member))]
        [ProducesResponseType(typeof(PaginatedList<BorrowRecordSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetMyPendingRequests")]
        public async Task<IActionResult> GetMyPendingRequests(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromServices] IUser currnetUser,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetMemberPendingRequestsQuery(currnetUser.Id!.Value, pageSize, pageNumber), cancellationToken);
            return result.Match(Ok, Problem);
        }


        [HttpGet("/api/v{version:apiVersion}/categories/{categoryId:guid}/borrow-records/overdues")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(typeof(PaginatedList<BorrowRecordSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetOverdueBorrowRecords")]
        public async Task<IActionResult> GetOverdueBorrowRecords(
            Guid categoryId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetOverdueBorrowingsQuery(categoryId, pageSize, pageNumber), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/categories/{categoryId:guid}/borrow-records/pendings")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(typeof(PaginatedList<BorrowRecordSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetWaitingsByCategory")]
        public async Task<IActionResult> GetWaitingsByCategory(
            Guid categoryId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetWaitingsByCategoryQuery(categoryId, pageSize, pageNumber), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/categories/{categoryId:guid}/borrow-records/pickup-ready")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(typeof(PaginatedList<BorrowRecordSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetReadyForPickup")]
        public async Task<IActionResult> GetReadyForPickup(
            Guid categoryId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new GetReadyForPickupQuery(categoryId, pageSize, pageNumber), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpPost("{id:guid}/cancellations")]
        [Authorize(Roles = nameof(Role.Member))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("CancelBorrowRecord")]
        public async Task<IActionResult> CancelBorrowRecord(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new CancelBorrowRecordCommand(id), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPost("{id:guid}/overdue-states")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("MarkRecordAsLate")]
        public async Task<IActionResult> MarkRecordAsLate(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new MarkRecordAsLateCommand(id), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{id:guid}/due-date")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("OverrideDueDate")]
        public async Task<IActionResult> OverrideDueDate(
            Guid id,
            [FromBody] OverrideDueDateRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new OverrideDueDateCommand(id, request.DueDate), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPost("{id:guid}/rejections")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("RejectBorrowRecord")]
        public async Task<IActionResult> RejectBorrowRecord(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new RejectBorrowRecordCommand(id), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPost("{id:guid}/renewals")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("RenewBook")]
        public async Task<IActionResult> RenewBook(
            Guid id,
            [FromBody] RenewBookRequest request,
            [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new RenewBookCommand(id, request.DueDate, idempotencyKey), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }
    }
}
