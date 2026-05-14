namespace Lms.Api.Dtos.Requests
{
    public record UpdateBookThemesRequest
    (
        List<Guid> ThemeIds
    );
}
