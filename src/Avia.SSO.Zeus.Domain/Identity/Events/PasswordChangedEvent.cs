using Avia.SSO.Zeus.Domain.Common;

namespace Avia.SSO.Zeus.Domain.Identity.Events;

public sealed record PasswordChangedEvent(Guid UserId, Guid TenantId) : DomainEvent;
