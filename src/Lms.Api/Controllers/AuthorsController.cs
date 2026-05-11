using Asp.Versioning;
using Lms.Application.Features.Authors.Commands.CreateAuthor;
using Lms.Application.Features.Authors.Commands.DeleteAuthor;
using Lms.Application.Features.Authors.Commands.UpdateAuthor;
using Lms.Application.Features.Authors.Dtos;
using Lms.Application.Features.Authors.Queries.GetAuthors;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/authors")]
    public class AuthorsController(ISender sender) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<AuthorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all authors.")]
        [EndpointDescription("Returns all authors.")]
        [EndpointName("GetAuthors")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        [OutputCache(Duration = 60)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetAuthorsQuery(), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [EndpointSummary("Creates a new author.")]
        [EndpointDescription("This endpoint takes the name of an author, creates it, and returns its ID.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromHeader(Name="X-Idempotency-Key")] string idempotencyKey, [FromBody] string name, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateAuthorCommand(name, idempotencyKey), cancellationToken);
            return result.Match(result => StatusCode(StatusCodes.Status201Created, result), Problem);
        }

        [HttpDelete("{authorId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Delete an author.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid authorId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteAuthorCommand(authorId), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{authorId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Update an author.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid authorId, [FromBody] string name, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateAuthorCommand(authorId, name), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }
    }
}
