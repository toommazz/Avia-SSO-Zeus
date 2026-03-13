using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Identity.Enums;

namespace Avia.SSO.Zeus.Domain.Identity.Events;

public sealed record UserLoginFailedEvent(Guid UserId, Guid TenantId, LoginFailureReason Reason) : DomainEvent;
