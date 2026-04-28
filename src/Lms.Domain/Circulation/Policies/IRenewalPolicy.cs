using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation.Policies
{
    public interface IRenewalPolicy
    {
        Result<Success> Evaluate(BorrowStats stats);
    }
}
