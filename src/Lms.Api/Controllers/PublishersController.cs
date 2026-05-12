using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Features.Publishers.Commands.CreatePublisher;
using Lms.Application.Features.Publishers.Commands.DeletePublisher;
using Lms.Application.Features.Publishers.Commands.UpdatePublisher;
using Lms.Application.Features.Publishers.Dtos;
using Lms.Application.Features.Publishers.Queries.GetPublishers;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/publishers")]
    public class PublishersController(ISender sender) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<PublisherDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all publishers.")]
        [EndpointDescription("Returns all publishers.")]
        [EndpointName("GetPublishers")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        [OutputCache(Duration = 60)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetPublishersQuery(), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [EndpointSummary("Creates a new publisher.")]
        [EndpointDescription("This endpoint takes the name of an publisher, creates it, and returns its ID.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromHeader(Name="X-Idempotency-Key")] string idempotencyKey, [FromBody] CreatePublisherRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreatePublisherCommand(request.Name, idempotencyKey), cancellationToken);
            return result.Match(result => StatusCode(StatusCodes.Status201Created, result), Problem);
        }

        [HttpDelete("{publisherId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Delete an publisher.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid publisherId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeletePublisherCommand(publisherId), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{publisherId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Update an publisher.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid publisherId, [FromBody] CreatePublisherRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdatePublisherCommand(publisherId, request.Name), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }
    }
}
