using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;
using Avia.SSO.Zeus.Domain.Session.Errors;
using Avia.SSO.Zeus.Domain.Session.Events;
using Avia.SSO.Zeus.Domain.Session.ValueObjects;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Session.Aggregates;

public sealed class AuthSession : AggregateRoot
{
    public SessionId SessionId { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public TenantId TenantId { get; private set; } = null!;
    public DeviceInfo DeviceInfo { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    private AuthSession() { }

    public static Result<AuthSession> Create(Guid userId, Guid tenantId, DeviceInfo deviceInfo, int expirationMinutes = 60)
    {
        var tenantIdResult = TenantId.Create(tenantId);
        if (tenantIdResult.IsFailure)
            return Result.Failure<AuthSession>(tenantIdResult.Error);

        var sessionId = SessionId.New();
        var session = new AuthSession
        {
            Id = sessionId.Value,
            SessionId = sessionId,
            UserId = userId,
            TenantId = tenantIdResult.Value,
            DeviceInfo = deviceInfo,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        session.RaiseDomainEvent(new SessionCreatedEvent(sessionId.Value, userId, tenantId));
        return Result.Success(session);
    }

    public Result Revoke()
    {
        if (IsRevoked)
            return Result.Failure(SessionErrors.AlreadyRevoked);

        IsRevoked = true;
        RaiseDomainEvent(new SessionRevokedEvent(SessionId.Value, UserId, TenantId.Value));
        return Result.Success();
    }
}
