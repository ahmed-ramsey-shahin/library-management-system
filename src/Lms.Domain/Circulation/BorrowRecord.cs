using Lms.Domain.Common;

namespace Lms.Domain.Circulation
{
    public sealed class BorrowRecord : AuditableEntity
    {
        public Guid Id { get; }

        private BorrowRecord()
        {}
    }
}
