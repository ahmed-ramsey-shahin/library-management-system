using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation.Policies
{
    public class MaxActiveBorrowsPolicy(int maxActiveBorrows) : IBorrowPolicy
    {
        public Result<Success> Evaluate(BorrowStats stats)
        {
            if (stats.ActiveBorrows >= maxActiveBorrows)
            {
                return BorrowPolicyErrors.MaximumActiveBorrowsReached;
            }

            return Result.Success;
        }
    }
}
