using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Features.Keywords.Commands.CreateKeyword;
using Lms.Application.Features.Keywords.Commands.DeleteKeyword;
using Lms.Application.Features.Keywords.Commands.UpdateKeyword;
using Lms.Application.Features.Keywords.Dtos;
using Lms.Application.Features.Keywords.Queries.GetKeywords;
using Lms.Application.Features.Keywords.Queries.GetKeywordsByBookId;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/keywords")]
    public class KeywordsController(ISender sender) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<KeywordDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all keywords.")]
        [EndpointDescription("Returns all keywords.")]
        [EndpointName("GetKeywords")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        [OutputCache(Duration = 60)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetKeywordsQuery(), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [EndpointSummary("Creates a new keyword.")]
        [EndpointDescription("This endpoint takes the name of an keyword, creates it, and returns its ID.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromHeader(Name="X-Idempotency-Key")] string idempotencyKey, [FromBody] CreateKeywordRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateKeywordCommand(request.Name, idempotencyKey), cancellationToken);
            return result.Match(result => StatusCode(StatusCodes.Status201Created, result), Problem);
        }

        [HttpDelete("{keywordId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Delete an keyword.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid keywordId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteKeywordCommand(keywordId), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{keywordId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Update an keyword.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid keywordId, [FromBody] CreateKeywordRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateKeywordCommand(keywordId, request.Name), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/books/{bookId:guid}/keywords")]
        [ProducesResponseType(typeof(List<KeywordDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetKeywordsByBook")]
        public async Task<IActionResult> GetKeywordsByBook(Guid bookId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetKeywordsByBookIdQuery(bookId), cancellationToken);
            return result.Match(Ok, Problem);
        }
    }
}
