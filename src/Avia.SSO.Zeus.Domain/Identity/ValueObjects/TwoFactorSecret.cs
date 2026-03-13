using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Identity.ValueObjects;

public sealed class TwoFactorSecret : ValueObject
{
    public string Value { get; }

    private TwoFactorSecret(string value) => Value = value;

    public static Result<TwoFactorSecret> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<TwoFactorSecret>(new Error("User.TwoFactorSecret.Empty", "Two-factor secret cannot be empty.", ErrorType.Validation));
        return Result.Success(new TwoFactorSecret(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
