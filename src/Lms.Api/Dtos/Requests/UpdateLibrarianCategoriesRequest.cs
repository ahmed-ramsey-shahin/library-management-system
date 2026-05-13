namespace Lms.Api.Dtos.Requests
{
    public record UpdateLibrarianCategoriesRequest(
        List<Guid> CategoryIds
    );
}
