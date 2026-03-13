using Avia.SSO.Zeus.Domain.Common;

namespace Avia.SSO.Zeus.Domain.Identity.Events;

public sealed record UserDeactivatedEvent(Guid UserId, Guid TenantId) : DomainEvent;
