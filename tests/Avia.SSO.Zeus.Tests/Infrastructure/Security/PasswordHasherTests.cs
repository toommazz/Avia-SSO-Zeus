using Avia.SSO.Zeus.Infrastructure.Security;

namespace Avia.SSO.Zeus.Tests.Infrastructure.Security;

[TestClass]
public sealed class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [TestMethod]
    public void Hash_ShouldReturnNonEmptyHashAndSalt()
    {
        var (hash, salt) = _hasher.Hash("MyPassword@123");
        Assert.IsFalse(string.IsNullOrEmpty(hash));
        Assert.IsFalse(string.IsNullOrEmpty(salt));
    }

    [TestMethod]
    public void Hash_SamePlainPassword_ShouldProduceDifferentHashes()
    {
        var (hash1, _) = _hasher.Hash("MyPassword@123");
        var (hash2, _) = _hasher.Hash("MyPassword@123");
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void Verify_WithCorrectPassword_ShouldReturnTrue()
    {
        var (hash, salt) = _hasher.Hash("MyPassword@123");
        var isValid = _hasher.Verify("MyPassword@123", hash, salt);
        Assert.IsTrue(isValid);
    }

    [TestMethod]
    public void Verify_WithWrongPassword_ShouldReturnFalse()
    {
        var (hash, salt) = _hasher.Hash("MyPassword@123");
        var isValid = _hasher.Verify("WrongPassword!", hash, salt);
        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void Verify_WithTamperedHash_ShouldReturnFalse()
    {
        var (_, salt) = _hasher.Hash("MyPassword@123");
        // Use a valid base64 string that is simply the wrong hash value
        var tamperedHash = Convert.ToBase64String(new byte[32]);
        var isValid = _hasher.Verify("MyPassword@123", tamperedHash, salt);
        Assert.IsFalse(isValid);
    }
}
