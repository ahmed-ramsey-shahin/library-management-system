namespace Lms.Api.Dtos.Requests
{
    public record BorrowBookRequest
    (
        Guid BookId,
        DateOnly DueDate,
        DateOnly PickupDeadline
    );
}
