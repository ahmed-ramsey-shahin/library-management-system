using Asp.Versioning;
using Lms.Api.Dtos.Requests;
using Lms.Application.Features.Categories.Commands.CreateCategory;
using Lms.Application.Features.Categories.Commands.DeleteCategory;
using Lms.Application.Features.Categories.Commands.UpdateCategory;
using Lms.Application.Features.Categories.Dtos;
using Lms.Application.Features.Categories.Queries.GetCategories;
using Lms.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Lms.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/categories")]
    public class CategoriesController(ISender sender) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves all categories.")]
        [EndpointDescription("Returns all categories.")]
        [EndpointName("GetCategories")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        [OutputCache(Duration = 60)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetCategoriesQuery(), cancellationToken);
            return result.Match(Ok, Problem);
        }

        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [EndpointSummary("Creates a new category.")]
        [EndpointDescription("This endpoint takes the name of an category, creates it, and returns its ID.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromHeader(Name="X-Idempotency-Key")] string idempotencyKey, [FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateCategoryCommand(request.Name, idempotencyKey), cancellationToken);
            return result.Match(result => StatusCode(StatusCodes.Status201Created, result), Problem);
        }

        [HttpDelete("{categoryId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Delete an category.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid categoryId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new DeleteCategoryCommand(categoryId), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpPut("{categoryId:guid}")]
        [Authorize(Roles = $"{nameof(Role.Librarian)},{nameof(Role.Admin)}")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Update an category.")]
        [MapToApiVersion("1.0")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid categoryId, [FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateCategoryCommand(categoryId, request.Name), cancellationToken);
            return result.Match(_ => NoContent(), Problem);
        }

        [HttpGet("/api/v{version:apiVersion}/books/{bookId:guid}/categories")]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [MapToApiVersion("1.0")]
        [EndpointName("GetCategoriesByBook")]
        public async Task<IActionResult> GetCategoriesByBook(Guid bookId, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetCategoriesByBookIdQuery(bookId), cancellationToken);
            return result.Match(Ok, Problem);
        }
    }
}
