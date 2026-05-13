using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Common.Interfaces;
using Lms.Application.Features.Users.Commands.ActivateUser;
using Lms.Application.Features.Users.Commands.AdminResetUserPassword;
using Lms.Application.Features.Users.Commands.ChangeUserDetails;
using Lms.Application.Features.Users.Commands.ChangeUserPassword;
using Lms.Application.Features.Users.Commands.CreateAdmin;
using Lms.Application.Features.Users.Commands.CreateLibrarian;
using Lms.Application.Features.Users.Commands.CreateMember;
using Lms.Application.Features.Users.Commands.DeleteUser;
using Lms.Application.Features.Users.Commands.SuspendUser;
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
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
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
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Librarian)}")]
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

        [HttpPost("{id:guid}/activation")]
        [Authorize(Roles = nameof(Role.Admin))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("ActivateUser")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ActivateUser(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new ActivateUserCommand(id), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{id:guid}/password-resets")]
        [Authorize(Roles = nameof(Role.Admin))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointName("AdminResetUserPassword")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AdminResetUserPassword(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new AdminResetUserPasswordCommand(id), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("me/password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [EndpointName("ChangeUserPassword")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ChangeUserPassword([FromServices] IUser userService, [FromBody] ChangeUserPasswordRequest request, CancellationToken cancellationToken)
        {
            if (userService.Id is null)
            {
                return Unauthorized();
            }

            var result = await sender.Send(new ChangeUserPasswordCommand(userService.Id.Value, request.OldPassword, request.NewPassword), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("ChangeUserDetails")]
        public async Task<IActionResult> ChangeUserDetails([FromServices] IUser userService, [FromBody] ChangeUserDetailsRequest request, CancellationToken cancellationToken)
        {
            if (userService.Id is null)
            {
                return Unauthorized();
            }

            var result = await sender.Send(
                new ChangeUserDetailsCommand(
                    userService.Id.Value,
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber,
                    request.Address
                ),
                cancellationToken
            );
            return result.Match(_ => NoContent(), Problem);
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [MapToApiVersion("1.0")]
        [EndpointName("DeleteUser")]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteUserCommand(id), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPost("{id:guid}/suspensions")]
        [Authorize(Roles = nameof(Role.Admin))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [MapToApiVersion("1.0")]
        [EndpointName("SuspendUser")]
        public async Task<IActionResult> SuspendUser(Guid id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new SuspendUserCommand(id), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }
    }
}
