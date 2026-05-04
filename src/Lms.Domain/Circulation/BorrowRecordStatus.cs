namespace Lms.Domain.Circulation
{
    public enum BorrowRecordStatus
    {
        Waiting,
        Accepted,
        Rejected,
        Late,
        Returned,
        Canceled,
        Lost,
    }
}
