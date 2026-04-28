using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation.Policies
{
    public class MaxLateBorrowsPolicy(int maxLateBorrows) : IBorrowPolicy, IRenewalPolicy
    {
        public Result<Success> Evaluate(BorrowStats stats)
        {
            if (stats.LateBorrows >= maxLateBorrows)
            {
                return BorrowPolicyErrors.MaximumLateBorrowsReached;
            }

            return Result.Success;
        }
    }
}
