namespace Lms.Api.Dtos.Requests
{
    public record UpdateBookKeywordsRequest
    (
        List<Guid> KeywordIds
    );
}
