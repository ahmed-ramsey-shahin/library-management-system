namespace Lms.Api.Dtos.Requests
{
    public record UpdateBookDetailsRequest
    (
        string? Isbn,
        string? Issn,
        string? Title,
        string? Description,
        int? PageCount,
        Guid? PublisherId,
        DateOnly? PublishingDate,
        string? Edition,
        string? Language
    );
}
