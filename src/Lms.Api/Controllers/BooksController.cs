using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Common.Models;
using Lms.Application.Features.Books.Commands.CreateBook;
using Lms.Application.Features.Books.Commands.DeleteBook;
using Lms.Application.Features.Books.Commands.UpdateBookAudiences;
using Lms.Application.Features.Books.Commands.UpdateBookAuthors;
using Lms.Application.Features.Books.Commands.UpdateBookCategories;
using Lms.Application.Features.Books.Commands.UpdateBookDetails;
using Lms.Application.Features.Books.Commands.UpdateBookFinancials;
using Lms.Application.Features.Books.Commands.UpdateBookGenres;
using Lms.Application.Features.Books.Commands.UpdateBookKeywords;
using Lms.Application.Features.Books.Commands.UpdateBookThemes;
using Lms.Application.Features.Books.Dtos;
using Lms.Application.Features.Books.Queries.GetBookById;
using Lms.Application.Features.Books.Queries.GetBooksByAudience;
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

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [MapToApiVersion("1.0")]
        [EndpointName("DeleteBook")]
        public async Task<IActionResult> DeleteBook(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteBookCommand(id), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{id:guid}/audiences")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointName("UpdateBookAudiences")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateBookAudiences(Guid id, [FromBody] UpdateBookAudiencesRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateBookAudiencesCommand(id, request.AudienceIds), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{id:guid}/categories")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointName("UpdateBookCategories")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateBookCategories(Guid id, [FromBody] UpdateBookCategoriesRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateBookCategoriesCommand(id, request.CategoryIds), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{id:guid}/details")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointName("UpdateBookDetails")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateBookDetails(Guid id, [FromBody] UpdateBookDetailsRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateBookDetailsCommand(
                id,
                request.Isbn,
                request.Issn,
                request.Title,
                request.Description,
                request.PageCount,
                request.PublisherId,
                request.PublishingDate,
                request.Edition,
                request.Language
            ), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{id:guid}/authors")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointName("UpdateBookAuthors")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateBookAuthors(Guid id, [FromBody] UpdateBookAuthorsRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateBookAuthorsCommand(id, request.AuthorIds), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{id:guid}/genres")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointName("UpdateBookGenres")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateBookGenres(Guid id, [FromBody] UpdateBookGenresRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateBookGenresCommand(id, request.GenreIds), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{id:guid}/keywords")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointName("UpdateBookKeywords")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateBookKeywords(Guid id, [FromBody] UpdateBookKeywordsRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateBookKeywordsCommand(id, request.KeywordIds), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{id:guid}/themes")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointName("UpdateBookThemes")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateBookThemes(Guid id, [FromBody] UpdateBookThemesRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateBookThemesCommand(id, request.ThemeIds), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{id:guid}/financials")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointName("UpdateBookFinancials")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateBookFinancials(Guid id, [FromBody] UpdateBookFinancialsRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateBookFinancialsCommand(
                id,
                request.BorrowPricePerDay,
                request.FinePerDay,
                request.LostFee,
                request.DamageFee
            ), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/audiences/{audienceId:guid}/books")]
        [ProducesResponseType(typeof(PaginatedList<BookDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetBooksByAudience")]
        public async Task<IActionResult> GetBooksByAudience(Guid audienceId, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetBooksByAudienceQuery(audienceId, pageSize, pageNumber), cancellationToken);
            return result.Match(Ok, Problem);
        }
    }
}
