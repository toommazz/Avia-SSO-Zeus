using Avia.SSO.Zeus.Domain.Session.Aggregates;
using Avia.SSO.Zeus.Domain.Session.Events;
using Avia.SSO.Zeus.Domain.Session.ValueObjects;

namespace Avia.SSO.Zeus.Tests.Domain.Session;

[TestClass]
public sealed class AuthSessionTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid TenantId = Guid.NewGuid();

    [TestMethod]
    public void Create_WithValidData_ShouldSucceed()
    {
        var deviceInfo = DeviceInfo.Create("Mozilla/5.0", "127.0.0.1");
        var result = AuthSession.Create(UserId, TenantId, deviceInfo);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Value.IsActive);
        Assert.IsFalse(result.Value.IsRevoked);
    }

    [TestMethod]
    public void Create_ShouldRaiseSessionCreatedEvent()
    {
        var deviceInfo = DeviceInfo.Create("Mozilla/5.0", "127.0.0.1");
        var result = AuthSession.Create(UserId, TenantId, deviceInfo);
        Assert.IsInstanceOfType<SessionCreatedEvent>(result.Value.DomainEvents.First());
    }

    [TestMethod]
    public void Revoke_ShouldSetRevokedAndRaiseEvent()
    {
        var deviceInfo = DeviceInfo.Create("Mozilla/5.0", "127.0.0.1");
        var session = AuthSession.Create(UserId, TenantId, deviceInfo).Value;
        session.ClearDomainEvents();

        var result = session.Revoke();

        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(session.IsRevoked);
        Assert.IsFalse(session.IsActive);
        Assert.IsInstanceOfType<SessionRevokedEvent>(session.DomainEvents.First());
    }

    [TestMethod]
    public void Revoke_AlreadyRevoked_ShouldFail()
    {
        var deviceInfo = DeviceInfo.Create("Mozilla/5.0", "127.0.0.1");
        var session = AuthSession.Create(UserId, TenantId, deviceInfo).Value;
        session.Revoke();

        var result = session.Revoke();

        Assert.IsTrue(result.IsFailure);
    }
}
