using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Features.Books.Commands.CreateBookCopy;
using Lms.Application.Features.Books.Commands.DeleteBookCopy;
using Lms.Application.Features.Books.Commands.MarkBookCopyAsAvailable;
using Lms.Application.Features.Books.Commands.MarkBookCopyAsBorrowed;
using Lms.Application.Features.Books.Commands.MarkBookCopyAsMaintenance;
using Lms.Application.Features.Books.Commands.UpdateBookCopyLocation;
using Lms.Application.Features.Books.Commands.UpdateBookCopyStatus;
using Lms.Application.Features.Books.Dtos;
using Lms.Application.Features.Books.Queries.GetBookCopyById;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/books/{bookId:guid}/copies")]
    public class BookCopiesController(ISender sender) : ApiController
    {
        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [EndpointName("CreateBookCopy")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreateBookCopy(
            Guid bookId,
            [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey,
            CreateBookCopyRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new CreateBookCopyCommand(
                bookId,
                request.Barcode,
                request.Status,
                request.State,
                request.Location,
                request.AcquisitionDate,
                idempotencyKey
            ), cancellationToken);
            return result.Match(
                id => CreatedAtAction(
                    nameof(GetBookCopyById),
                    new
                    {
                        id
                    },
                    id
                ),
                Problem
            );
        }

        [HttpGet("/api/v{version:apiVersion}/book-copies/{id:guid}")]
        [ProducesResponseType(typeof(BookCopyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetBookCopyById")]
        public async Task<IActionResult> GetBookCopyById(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetBookCopyByIdQuery(id), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpDelete("{copyId:guid}")]
        [Authorize(Roles = nameof(Role.Admin))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("DeleteBookCopy")]
        public async Task<IActionResult> DeleteBookCopy(
            Guid bookId,
            Guid copyId,
            [FromBody] DeleteBookCopyRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new DeleteBookCopyCommand(bookId, copyId, request.Version), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{copyId:guid}/location")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("ChangeBookCopyLocation")]
        public async Task<IActionResult> ChangeBookCopyLocation(
            Guid bookId,
            Guid copyId,
            [FromBody] ChangeBookCopyLocationRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new UpdateBookCopyLocationCommand(bookId, copyId, request.Location, request.Version), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{copyId:guid}/status")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("ChangeBookCopyStatus")]
        public async Task<IActionResult> ChangeBookCopyStatus(
            Guid bookId,
            Guid copyId,
            [FromBody] ChangeBookCopyStatusRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new UpdateBookCopyStatusCommand(bookId, copyId, request.Status, request.Version), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPost("{copyId:guid}/availability")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("MarkBookCopyAsAvailable")]
        public async Task<IActionResult> MarkBookCopyAsAvailable(
            Guid bookId,
            Guid copyId,
            [FromBody] MarkBookCopyAsAvailableRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new MarkBookCopyAsAvailableCommand(bookId, copyId, request.Version), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPost("{copyId:guid}/borrowings")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("MarkBookCopyAsBorrowed")]
        public async Task<IActionResult> MarkBookCopyAsBorrowed(
            Guid bookId,
            Guid copyId,
            [FromBody] MarkBookCopyAsBorrowedRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new MarkBookCopyAsBorrowedCommand(bookId, copyId, request.Version), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPost("{copyId:guid}/maintenance")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("MarkBookCopyAsMaintenance")]
        public async Task<IActionResult> MarkBookCopyAsMaintenance(
            Guid bookId,
            Guid copyId,
            [FromBody] MarkBookCopyAsMaintenanceRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new MarkBookCopyAsMaintenanceCommand(bookId, copyId, request.Version), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }
    }
}
