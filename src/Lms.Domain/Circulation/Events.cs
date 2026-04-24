using Lms.Domain.Common;

namespace Lms.Domain.Circulation;

public sealed record BorrowRequestAcceptedEvent(Guid BorrowRequestId) : DomainEvent;
public sealed record BorrowRequestRejectedEvent(Guid BorrowRequestId) : DomainEvent;
public sealed record BookReturnedEvent(Guid BorrowRecordId) : DomainEvent;
public sealed record BookRenewedEvent(Guid BorrowRecordId) : DomainEvent;
public sealed record BorrowRecordMarkedAsLateEvent(Guid BorrowRecordId) : DomainEvent;
public sealed record FinePaidDomainEvent(Guid FineId, Guid BorrowRecordId) : DomainEvent;
