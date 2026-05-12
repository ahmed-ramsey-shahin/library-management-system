using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Commands.CreateAdmin;
using Lms.Application.Features.Users.Commands.CreateLibrarian;
using Lms.Application.Features.Users.Commands.CreateMember;
using Lms.Application.Features.Users.Dtos;
using Lms.Application.Features.Users.Queries.GetAdminById;
using Lms.Application.Features.Users.Queries.GetLibrarianById;
using Lms.Application.Features.Users.Queries.GetMemberById;
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
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
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

        [HttpGet("librarians/{id:guid}")]
        [Authorize(Roles = nameof(Role.Admin))]
        [ProducesResponseType(typeof(LibrarianDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetLibrarianById")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetLibrarianById(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetLibrarianByIdQuery(id), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpPost("librarians")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [EndpointName("CreateLibrarian")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> CreateLibrarian([FromBody] CreateLibrarianRequest request, [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey,CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateLibrarianCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Address,
                request.Password,
                request.CategoryIds,
                idempotencyKey
            ), cancellationToken);
            return result.Match(
                id => CreatedAtAction(
                    nameof(GetLibrarianById),
                    new {
                        id,
                    },
                    id
                ),
                Problem
            );
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LibrarianDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetCurrentUser")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCurrentUser([FromServices] IUser userService)
        {
            if (userService.Id is null)
            {
                return Unauthorized();
            }

            var userId = userService.Id.Value;
            return userService.UserRole switch
            {
                Role.Admin => (await sender.Send(new GetAdminByIdQuery(userId))).Match(Ok, Problem),
                Role.Librarian => (await sender.Send(new GetLibrarianByIdQuery(userId))).Match(Ok, Problem),
                Role.Member => (await sender.Send(new GetMemberByIdQuery(userId))).Match(Ok, Problem),
                _ => Unauthorized()
            };
        }

        [HttpPost("members")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Member)}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [EndpointName("CreateMember")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreateMember([FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey, [FromBody] CreateMemberRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateMemberCommand(
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
                    nameof(GetMemberById),
                    new {
                        id,
                    },
                    id
                ),
                Problem
            );
        }

        [HttpGet("members/{id:guid}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Member)}")]
        [ProducesResponseType(typeof(MemberDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetMemberById")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetMemberById(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetMemberByIdQuery(id), cancellationToken);
            return result.Match(Ok, Problem);
        }
    }
}
