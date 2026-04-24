using Lms.Domain.Common;

namespace Lms.Domain.Identity;

public sealed record UserSuspended(Guid UserId) : DomainEvent;
public sealed record UserActivated(Guid UserId) : DomainEvent;
public sealed record CategoryAdded(Guid UserId, Guid CategoryId) : DomainEvent;
public sealed record CategoryRemoved(Guid UserId, Guid CategoryId) : DomainEvent;
