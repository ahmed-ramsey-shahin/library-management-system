using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Features.Users.Commands.CreateAdmin;
using Lms.Application.Features.Users.Dtos;
using Lms.Application.Features.Users.Queries.GetAdminById;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController(ISender sender) : ApiController
    {
        [HttpPost("admins")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [EndpointName("CreateAdmin")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request, [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey,CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateAdminCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Address,
                request.Password,
                idempotencyKey
            ), cancellationToken);
            return result.Match(
                id => CreatedAtAction(
                    nameof(GetAdminById),
                    new {
                        id,
                    },
                    id
                ),
                Problem
            );
        }

        [HttpGet("admins/{id:guid}")]
        [Authorize(Roles = nameof(Role.Admin))]
        [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetAdminById")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAdminById(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetAdminByIdQuery(id), cancellationToken);
            return result.Match(Ok, Problem);
        }
    }
}
