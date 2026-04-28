using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation.Policies
{
    public static class BorrowPolicyErrors
    {
        public static Error MaximumLateBorrowsReached => Error.Forbidden("BorrowPolicy.MaximumLateBorrowsReached", "You have reached the maximum number of late borrows.");
        public static Error MaximumActiveBorrowsReached => Error.Forbidden("BorrowPolicy.MaximumActiveBorrowsReached", "You have reached the maximum number of active borrows.");
        public static Error MaximumRenewalCountReached => Error.Forbidden("BorrowPolicy.MaximumRenewalCountReached", "You have reached the maximum number of renewals.");
        public static Error MaximumUnpaidFinesReached => Error.Forbidden("BorrowPolicy.MaximumUnpaidFinesReached", "You have reached the maximum number of unpaid fines.");
    }
}
