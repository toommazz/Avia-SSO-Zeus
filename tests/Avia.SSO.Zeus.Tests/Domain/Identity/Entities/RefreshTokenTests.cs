using Avia.SSO.Zeus.Domain.Identity.Entities;

namespace Avia.SSO.Zeus.Tests.Domain.Identity.Entities;

[TestClass]
public sealed class RefreshTokenTests
{
    [TestMethod]
    public void Create_ShouldBeActiveAndNotRevoked()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "mytoken", DateTime.UtcNow.AddDays(7));
        Assert.IsTrue(token.IsActive);
        Assert.IsFalse(token.IsRevoked);
        Assert.IsFalse(token.IsExpired);
    }

    [TestMethod]
    public void Create_WithPastExpiry_ShouldBeExpired()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "mytoken", DateTime.UtcNow.AddSeconds(-1));
        Assert.IsTrue(token.IsExpired);
        Assert.IsFalse(token.IsActive);
    }

    [TestMethod]
    public void Revoke_ShouldSetIsRevokedAndInactive()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "mytoken", DateTime.UtcNow.AddDays(7));
        token.Revoke();
        Assert.IsTrue(token.IsRevoked);
        Assert.IsFalse(token.IsActive);
    }
}
