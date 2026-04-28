namespace Lms.Application.Common.Configurations
{
    public class BorrowSettings
    {
        public int MaxActiveBorrows { get; set; }
        public int MaxLateBorrows { get; set; }
        public int MaxUnpaidFines { get; set; }
        public int MaxRenewalCount { get; set; }
    }
}
