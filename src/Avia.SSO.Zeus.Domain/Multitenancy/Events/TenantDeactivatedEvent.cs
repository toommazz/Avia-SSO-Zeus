using Avia.SSO.Zeus.Domain.Common;

namespace Avia.SSO.Zeus.Domain.Multitenancy.Events;

public sealed record TenantDeactivatedEvent(Guid TenantId) : DomainEvent;
