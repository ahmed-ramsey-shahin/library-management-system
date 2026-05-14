namespace Lms.Api.Dtos.Requests
{
    public record UpdateBookCategoriesRequest
    (
        List<Guid> CategoryIds
    );
}
