namespace Lms.Api.Dtos.Requests
{
    public record UpdateBookAuthorsRequest
    (
        List<Guid> AuthorIds
    );
}
