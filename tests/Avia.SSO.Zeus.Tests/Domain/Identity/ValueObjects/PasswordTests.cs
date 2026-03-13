using Avia.SSO.Zeus.Domain.Identity.ValueObjects;

namespace Avia.SSO.Zeus.Tests.Domain.Identity.ValueObjects;

[TestClass]
public sealed class PasswordTests
{
    [TestMethod]
    public void Create_WithValidHashAndSalt_ShouldSucceed()
    {
        var result = Password.Create("hashedvalue", "saltvalue");
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("hashedvalue", result.Value.Hash);
        Assert.AreEqual("saltvalue", result.Value.Salt);
    }

    [TestMethod]
    public void Create_WithEmptyHash_ShouldFail()
    {
        var result = Password.Create("", "saltvalue");
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void Create_WithEmptySalt_ShouldFail()
    {
        var result = Password.Create("hashedvalue", "");
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void From_ShouldCreatePasswordDirectly()
    {
        var password = Password.From("hash", "salt");
        Assert.AreEqual("hash", password.Hash);
        Assert.AreEqual("salt", password.Salt);
    }
}
