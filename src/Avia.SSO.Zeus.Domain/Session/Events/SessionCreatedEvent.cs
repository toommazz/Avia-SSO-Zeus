using Avia.SSO.Zeus.Domain.Common;

namespace Avia.SSO.Zeus.Domain.Session.Events;

public sealed record SessionCreatedEvent(Guid SessionId, Guid UserId, Guid TenantId) : DomainEvent;
