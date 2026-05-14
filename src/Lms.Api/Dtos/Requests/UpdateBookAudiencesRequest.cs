namespace Lms.Api.Dtos.Requests
{
    public record UpdateBookAudiencesRequest
    (
        List<Guid> AudienceIds
    );
}
