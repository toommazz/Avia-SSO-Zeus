using Avia.SSO.Zeus.Domain.Common;

namespace Avia.SSO.Zeus.Domain.Identity.Events;

public sealed record UserLoginSucceededEvent(Guid UserId, Guid TenantId, DateTime OccurredOn) : DomainEvent;
