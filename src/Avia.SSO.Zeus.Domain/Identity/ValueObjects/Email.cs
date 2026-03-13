using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Identity.Errors;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Identity.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Email>(UserErrors.Email.Empty);
        if (!value.Contains('@'))
            return Result.Failure<Email>(UserErrors.Email.InvalidFormat);
        return Result.Success(new Email(value.ToLowerInvariant().Trim()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
