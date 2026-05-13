using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Features.Books.Commands.CreateBook;
using Lms.Application.Features.Books.Dtos;
using Lms.Application.Features.Books.Queries.GetBookById;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/books")]
    public class BooksController(ISender sender) : ApiController
    {
        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateBook")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreateBook(
            [FromBody] CreateBookRequest request,
            [FromHeader(Name = "X-Idempotency-Key")] [Required] string idempotencyKey,
            CancellationToken cancellationToken
        )
        {
            var result = await sender.Send(new CreateBookCommand
            {
                CategoryIds = request.CategoryIds,
                Description = request.Description,
                AudienceIds = request.AudienceIds,
                AuthorIds = request.AuthorIds,
                BorrowPricePerDay = request.BorrowPricePerDay,
                DamageFee = request.DamageFee,
                Edition = request.Edition,
                FinePerDay = request.FinePerDay,
                GenreIds = request.GenreIds,
                IdempotencyKey = idempotencyKey,
                Isbn = request.Isbn,
                Issn = request.Issn,
                KeywordIds = request.KeywordIds,
                Language = request.Language,
                LostFee = request.LostFee,
                PageCount = request.PageCount,
                PublisherId = request.PublisherId,
                PublishingDate = request.PublishingDate,
                ThemeIds = request.ThemeIds,
                Title = request.Title,
            }, cancellationToken);
            return result.Match(id => CreatedAtAction(
                nameof(GetBookById),
                new
                {
                    id,
                },
                id
            ), Problem);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetBookById")]
        public async Task<IActionResult> GetBookById(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetBookByIdQuery(id), cancellationToken);
            return result.Match(Ok, Problem);
        }
    }
}
