using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Identity.Entities;
using Avia.SSO.Zeus.Domain.Identity.Enums;
using Avia.SSO.Zeus.Domain.Identity.Errors;
using Avia.SSO.Zeus.Domain.Identity.Events;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Identity.Aggregates;

public sealed class User : AggregateRoot
{
    private const int MaxFailedAttempts = 5;
    private readonly List<LoginAttempt> _loginAttempts = [];

    public UserId UserId { get; private set; } = null!;
    public TenantId TenantId { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    public TwoFactorSecret? TwoFactorSecret { get; private set; }
    public TwoFactorMethod TwoFactorMethod { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<LoginAttempt> LoginAttempts => _loginAttempts.AsReadOnly();

    private User() { }

    public static Result<User> Register(Guid tenantId, string email, string passwordHash, string passwordSalt)
    {
        var tenantIdResult = TenantId.Create(tenantId);
        if (tenantIdResult.IsFailure)
            return Result.Failure<User>(tenantIdResult.Error);

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure<User>(emailResult.Error);

        var passwordResult = Password.Create(passwordHash, passwordSalt);
        if (passwordResult.IsFailure)
            return Result.Failure<User>(passwordResult.Error);

        var userId = UserId.New();
        var user = new User
        {
            Id = userId.Value,
            UserId = userId,
            TenantId = tenantIdResult.Value,
            Email = emailResult.Value,
            Password = passwordResult.Value,
            TwoFactorMethod = TwoFactorMethod.None,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        user.RaiseDomainEvent(new UserRegisteredEvent(userId.Value, tenantId, emailResult.Value.Value));
        return Result.Success(user);
    }

    public static User Reconstitute(
        Guid id, Guid tenantId, string email,
        string passwordHash, string passwordSalt,
        string? twoFactorSecret, TwoFactorMethod twoFactorMethod,
        UserStatus status, DateTime createdAt)
    {
        var userId = UserId.From(id);
        return new User
        {
            Id = id,
            UserId = userId,
            TenantId = TenantId.Create(tenantId).Value,
            Email = Email.Create(email).Value,
            Password = Password.From(passwordHash, passwordSalt),
            TwoFactorSecret = twoFactorSecret is not null ? TwoFactorSecret.Create(twoFactorSecret).Value : null,
            TwoFactorMethod = twoFactorMethod,
            Status = status,
            CreatedAt = createdAt
        };
    }

    public Result ChangePassword(string newPasswordHash, string newPasswordSalt)
    {
        var passwordResult = Password.Create(newPasswordHash, newPasswordSalt);
        if (passwordResult.IsFailure)
            return Result.Failure(passwordResult.Error);

        Password = passwordResult.Value;
        RaiseDomainEvent(new PasswordChangedEvent(UserId.Value, TenantId.Value));
        return Result.Success();
    }

    public Result EnableTwoFactor(string secret, TwoFactorMethod method)
    {
        var secretResult = TwoFactorSecret.Create(secret);
        if (secretResult.IsFailure)
            return Result.Failure(secretResult.Error);

        TwoFactorSecret = secretResult.Value;
        TwoFactorMethod = method;
        return Result.Success();
    }

    public Result RecordLoginAttempt(bool succeeded, LoginFailureReason? failureReason = null, string? ipAddress = null)
    {
        if (Status == UserStatus.Locked)
            return Result.Failure(UserErrors.LockedOut);

        if (Status == UserStatus.Deactivated)
            return Result.Failure(UserErrors.AlreadyDeactivated);

        if (succeeded)
        {
            _loginAttempts.Add(LoginAttempt.Success(ipAddress));
            RaiseDomainEvent(new UserLoginSucceededEvent(UserId.Value, TenantId.Value, DateTime.UtcNow));
            return Result.Success();
        }

        var reason = failureReason ?? LoginFailureReason.InvalidPassword;
        _loginAttempts.Add(LoginAttempt.Failure(reason, ipAddress));
        RaiseDomainEvent(new UserLoginFailedEvent(UserId.Value, TenantId.Value, reason));

        var recentFailures = _loginAttempts
            .OrderByDescending(a => a.AttemptedAt)
            .TakeWhile(a => !a.Succeeded)
            .Count();

        if (recentFailures >= MaxFailedAttempts)
        {
            Status = UserStatus.Locked;
            RaiseDomainEvent(new UserLockedOutEvent(UserId.Value, TenantId.Value));
        }

        return Result.Success();
    }

    public Result Unlock()
    {
        Status = UserStatus.Active;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (Status == UserStatus.Deactivated)
            return Result.Failure(UserErrors.AlreadyDeactivated);

        Status = UserStatus.Deactivated;
        RaiseDomainEvent(new UserDeactivatedEvent(UserId.Value, TenantId.Value));
        return Result.Success();
    }
}
