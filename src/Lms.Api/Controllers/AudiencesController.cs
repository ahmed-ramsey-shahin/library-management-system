using Asp.Versioning;
using Lms.Application.Features.Audiences.Commands.CreateAudience;
using Lms.Application.Features.Audiences.Commands.DeleteAudience;
using Lms.Application.Features.Audiences.Commands.UpdateAudience;
using Lms.Application.Features.Audiences.Dtos;
using Lms.Application.Features.Audiences.Queries.GetAudiences;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/audiences")]
    public class AudiencesController(ISender sender) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<AudienceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all audiences.")]
        [EndpointDescription("Returns all audiences.")]
        [EndpointName("GetAudiences")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        [OutputCache(Duration = 60)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetAudiencesQuery(), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [EndpointSummary("Creates a new audience.")]
        [EndpointDescription("This endpoint takes the name of an audience, creates it, and returns its ID.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromHeader(Name="X-Idempotency-Key")] string idempotencyKey, [FromBody] string name, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateAudienceCommand(name, idempotencyKey), cancellationToken);
            return result.Match(result => StatusCode(StatusCodes.Status201Created, result), Problem);
        }

        [HttpDelete("{audienceId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Delete an audience.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid audienceId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteAudienceCommand(audienceId), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{audienceId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Update an audience.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid audienceId, [FromBody] string name, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateAudienceCommand(audienceId, name), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }
    }
}
