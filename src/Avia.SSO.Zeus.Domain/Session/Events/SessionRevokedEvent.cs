using Avia.SSO.Zeus.Domain.Common;

namespace Avia.SSO.Zeus.Domain.Session.Events;

public sealed record SessionRevokedEvent(Guid SessionId, Guid UserId, Guid TenantId) : DomainEvent;
