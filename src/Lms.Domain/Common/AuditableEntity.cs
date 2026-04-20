namespace Lms.Domain.Common
{
    public abstract class AuditableEntity
    {
        public DateTimeOffset CreatedAt { get; internal set; } = DateTimeOffset.UtcNow;
        public bool IsDeleted { get; internal set; }
    }
}
