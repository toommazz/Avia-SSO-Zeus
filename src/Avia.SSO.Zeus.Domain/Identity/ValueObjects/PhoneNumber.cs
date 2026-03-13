using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Identity.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<PhoneNumber>(new Error("User.PhoneNumber.Empty", "Phone number cannot be empty.", ErrorType.Validation));
        var digits = value.Replace("+", "").Replace("-", "").Replace(" ", "");
        if (!digits.All(char.IsDigit))
            return Result.Failure<PhoneNumber>(new Error("User.PhoneNumber.InvalidFormat", "Invalid phone number format.", ErrorType.Validation));
        return Result.Success(new PhoneNumber(value.Trim()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
