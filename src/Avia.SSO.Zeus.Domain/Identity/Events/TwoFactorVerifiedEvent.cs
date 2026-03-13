using Avia.SSO.Zeus.Domain.Common;

namespace Avia.SSO.Zeus.Domain.Identity.Events;

public sealed record TwoFactorVerifiedEvent(Guid UserId, Guid TenantId) : DomainEvent;
