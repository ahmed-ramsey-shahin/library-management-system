using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation.Policies
{
    public class MaxUnpaidFinesPolicy(int maxUnpaidFines) : IBorrowPolicy
    {
        public Result<Success> Evaluate(BorrowStats stats)
        {
            if (stats.UnpaidFines > maxUnpaidFines)
            {
                return BorrowPolicyErrors.MaximumUnpaidFinesReached;
            }

            return Result.Success;
        }
    }
}
