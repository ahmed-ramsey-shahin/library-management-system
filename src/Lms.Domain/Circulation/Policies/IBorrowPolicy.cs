using Lms.Domain.Common.Results;

namespace Lms.Domain.Circulation.Policies
{
    public interface IBorrowPolicy
    {
        Result<Success> Evaluate(BorrowStats stats);
    }
}
