using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation.Policies
{
    public class MaxRenewalCountPolicy(int maxRenewalCount) : IRenewalPolicy
    {
        public Result<Success> Evaluate(BorrowStats stats)
        {
            if (stats.RenewalCount >= maxRenewalCount)
            {
                return BorrowPolicyErrors.MaximumRenewalCountReached;
            }

            return Result.Success;
        }
    }
}
