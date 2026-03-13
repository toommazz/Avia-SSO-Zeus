using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Identity.Enums;

namespace Avia.SSO.Zeus.Domain.Identity.Events;

public sealed record TwoFactorRequestedEvent(Guid UserId, Guid TenantId, TwoFactorMethod Method) : DomainEvent;
