using Avia.SSO.Zeus.Domain.Common;

namespace Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;

public sealed class TenantSettings : ValueObject
{
    public bool TwoFactorRequired { get; }
    public int MaxLoginAttempts { get; }
    public int LockoutDurationMinutes { get; }

    private TenantSettings(bool twoFactorRequired, int maxLoginAttempts, int lockoutDurationMinutes)
    {
        TwoFactorRequired = twoFactorRequired;
        MaxLoginAttempts = maxLoginAttempts;
        LockoutDurationMinutes = lockoutDurationMinutes;
    }

    public static TenantSettings Default() => new(false, 5, 15);

    public static TenantSettings Create(bool twoFactorRequired, int maxLoginAttempts, int lockoutDurationMinutes)
        => new(twoFactorRequired, maxLoginAttempts, lockoutDurationMinutes);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TwoFactorRequired;
        yield return MaxLoginAttempts;
        yield return LockoutDurationMinutes;
    }
}
