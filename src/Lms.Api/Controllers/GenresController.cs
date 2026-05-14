using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Features.Genres.Commands.CreateGenre;
using Lms.Application.Features.Genres.Commands.DeleteGenre;
using Lms.Application.Features.Genres.Commands.UpdateGenre;
using Lms.Application.Features.Genres.Dtos;
using Lms.Application.Features.Genres.Queries.GetGenres;
using Lms.Application.Features.Genres.Queries.GetGenresByBookId;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/genres")]
    public class GenresController(ISender sender) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<GenreDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all genres.")]
        [EndpointDescription("Returns all genres.")]
        [EndpointName("GetGenres")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        [OutputCache(Duration = 60)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetGenresQuery(), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [EndpointSummary("Creates a new genre.")]
        [EndpointDescription("This endpoint takes the name of an genre, creates it, and returns its ID.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromHeader(Name="X-Idempotency-Key")] string idempotencyKey, [FromBody] CreateGenreRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateGenreCommand(request.Name, idempotencyKey), cancellationToken);
            return result.Match(result => StatusCode(StatusCodes.Status201Created, result), Problem);
        }

        [HttpDelete("{genreId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Delete an genre.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid genreId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteGenreCommand(genreId), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{genreId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Update an genre.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid genreId, [FromBody] CreateGenreRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateGenreCommand(genreId, request.Name), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/books/{bookId:guid}/genres")]
        [ProducesResponseType(typeof(List<GenreDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetGenresByBook")]
        public async Task<IActionResult> GetGenresByBook(Guid bookId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetGenresByBookIdQuery(bookId), cancellationToken);
            return result.Match(Ok, Problem);
        }
    }
}
