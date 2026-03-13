using Avia.SSO.Zeus.Domain.Common;

namespace Avia.SSO.Zeus.Domain.Identity.Events;

public sealed record UserRegisteredEvent(Guid UserId, Guid TenantId, string Email) : DomainEvent;
