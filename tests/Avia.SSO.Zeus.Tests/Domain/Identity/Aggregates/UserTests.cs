using Avia.SSO.Zeus.Domain.Identity.Aggregates;
using Avia.SSO.Zeus.Domain.Identity.Enums;
using Avia.SSO.Zeus.Domain.Identity.Events;

namespace Avia.SSO.Zeus.Tests.Domain.Identity.Aggregates;

[TestClass]
public sealed class UserTests
{
    private static readonly Guid TenantId = Guid.NewGuid();
    private const string ValidEmail = "user@example.com";
    private const string ValidHash = "hashedpassword";
    private const string ValidSalt = "saltyvalue";

    [TestMethod]
    public void Register_WithValidData_ShouldSucceed()
    {
        var result = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(ValidEmail, result.Value.Email.Value);
        Assert.AreEqual(UserStatus.Active, result.Value.Status);
    }

    [TestMethod]
    public void Register_ShouldRaiseUserRegisteredEvent()
    {
        var result = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt);
        var events = result.Value.DomainEvents;
        Assert.AreEqual(1, events.Count);
        Assert.IsInstanceOfType<UserRegisteredEvent>(events.First());
    }

    [TestMethod]
    public void Register_WithInvalidEmail_ShouldFail()
    {
        var result = User.Register(TenantId, "invalidemail", ValidHash, ValidSalt);
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void Register_WithEmptyTenantId_ShouldFail()
    {
        var result = User.Register(Guid.Empty, ValidEmail, ValidHash, ValidSalt);
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void ChangePassword_ShouldUpdatePasswordAndRaiseEvent()
    {
        var user = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt).Value;
        user.ClearDomainEvents();

        var result = user.ChangePassword("newhash", "newsalt");

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("newhash", user.Password.Hash);
        Assert.AreEqual(1, user.DomainEvents.Count);
        Assert.IsInstanceOfType<PasswordChangedEvent>(user.DomainEvents.First());
    }

    [TestMethod]
    public void RecordLoginAttempt_WithSuccess_ShouldRaiseLoginSucceededEvent()
    {
        var user = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt).Value;
        user.ClearDomainEvents();

        var result = user.RecordLoginAttempt(true);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsInstanceOfType<UserLoginSucceededEvent>(user.DomainEvents.First());
    }

    [TestMethod]
    public void RecordLoginAttempt_FiveFailures_ShouldLockUser()
    {
        var user = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt).Value;

        for (int i = 0; i < 5; i++)
            user.RecordLoginAttempt(false, LoginFailureReason.InvalidPassword);

        Assert.AreEqual(UserStatus.Locked, user.Status);
        Assert.IsTrue(user.DomainEvents.Any(e => e is UserLockedOutEvent));
    }

    [TestMethod]
    public void RecordLoginAttempt_WhenLocked_ShouldFail()
    {
        var user = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt).Value;
        for (int i = 0; i < 5; i++)
            user.RecordLoginAttempt(false, LoginFailureReason.InvalidPassword);

        var result = user.RecordLoginAttempt(true);
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void Unlock_ShouldSetStatusToActive()
    {
        var user = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt).Value;
        for (int i = 0; i < 5; i++)
            user.RecordLoginAttempt(false, LoginFailureReason.InvalidPassword);

        user.Unlock();

        Assert.AreEqual(UserStatus.Active, user.Status);
    }

    [TestMethod]
    public void Deactivate_ShouldSetStatusAndRaiseEvent()
    {
        var user = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt).Value;
        user.ClearDomainEvents();

        var result = user.Deactivate();

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(UserStatus.Deactivated, user.Status);
        Assert.IsInstanceOfType<UserDeactivatedEvent>(user.DomainEvents.First());
    }

    [TestMethod]
    public void Deactivate_AlreadyDeactivated_ShouldFail()
    {
        var user = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt).Value;
        user.Deactivate();

        var result = user.Deactivate();

        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void EnableTwoFactor_ShouldSetSecretAndMethod()
    {
        var user = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt).Value;

        var result = user.EnableTwoFactor("SECRETKEY", TwoFactorMethod.Totp);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(TwoFactorMethod.Totp, user.TwoFactorMethod);
        Assert.IsNotNull(user.TwoFactorSecret);
    }

    [TestMethod]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        var user = User.Register(TenantId, ValidEmail, ValidHash, ValidSalt).Value;
        Assert.AreEqual(1, user.DomainEvents.Count);

        user.ClearDomainEvents();

        Assert.AreEqual(0, user.DomainEvents.Count);
    }
}
