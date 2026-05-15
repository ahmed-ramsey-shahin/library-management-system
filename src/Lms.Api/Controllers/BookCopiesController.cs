using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Features.Books.Commands.CreateBookCopy;
using Lms.Application.Features.Books.Dtos;
using Lms.Application.Features.Books.Queries.GetBookCopyById;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/book-copies")]
    public class BookCopiesController(ISender sender) : ApiController
    {
        [HttpPost("/api/v{version:apiVersion}/books/{bookId:guid}/copies")]
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

        [HttpGet("{id:guid}")]
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
    }
}
