using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation.Policies
{
    public static class BorrowPolicyErrors
    {
        // --- Forbidden Errors (Policy Violations) ---
        public static Error MaximumLateBorrowsReached =>
            Error.Forbidden("BorrowPolicy.MaxLateBorrowsReached", "The member has reached the maximum allowed number of overdue borrows.");

        public static Error MaximumActiveBorrowsReached =>
            Error.Forbidden("BorrowPolicy.MaxActiveBorrowsReached", "The member has reached the maximum allowed number of active borrows.");

        public static Error MaximumRenewalCountReached =>
            Error.Forbidden("BorrowPolicy.MaxRenewalCountReached", "The maximum number of renewals for this item has been reached.");

        public static Error MaximumUnpaidFinesReached =>
            Error.Forbidden("BorrowPolicy.MaxUnpaidFinesReached", "The member has reached the maximum allowed number of unpaid fines.");
    }
}
