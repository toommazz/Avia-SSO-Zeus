using Avia.SSO.Zeus.Domain.Identity.ValueObjects;

namespace Avia.SSO.Zeus.Tests.Domain.Identity.ValueObjects;

[TestClass]
public sealed class EmailTests
{
    [TestMethod]
    public void Create_WithValidEmail_ShouldSucceed()
    {
        var result = Email.Create("user@example.com");
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("user@example.com", result.Value.Value);
    }

    [TestMethod]
    public void Create_ShouldNormalizeLowercase()
    {
        var result = Email.Create("USER@EXAMPLE.COM");
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("user@example.com", result.Value.Value);
    }

    [TestMethod]
    public void Create_WithEmptyEmail_ShouldFail()
    {
        var result = Email.Create("");
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void Create_WithoutAtSign_ShouldFail()
    {
        var result = Email.Create("invalidemail.com");
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void TwoEmails_WithSameValue_ShouldBeEqual()
    {
        var email1 = Email.Create("user@example.com").Value;
        var email2 = Email.Create("user@example.com").Value;
        Assert.AreEqual(email1, email2);
    }

    [TestMethod]
    public void TwoEmails_WithDifferentValues_ShouldNotBeEqual()
    {
        var email1 = Email.Create("a@example.com").Value;
        var email2 = Email.Create("b@example.com").Value;
        Assert.AreNotEqual(email1, email2);
    }
}
