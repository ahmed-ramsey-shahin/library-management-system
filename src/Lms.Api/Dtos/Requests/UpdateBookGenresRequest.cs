namespace Lms.Api.Dtos.Requests
{
    public record UpdateBookGenresRequest
    (
        List<Guid> GenreIds
    );
}
