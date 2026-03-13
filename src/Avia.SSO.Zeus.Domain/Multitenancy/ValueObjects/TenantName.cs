using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Multitenancy.Errors;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;

public sealed class TenantName : ValueObject
{
    public string Value { get; }

    private TenantName(string value) => Value = value;

    public static Result<TenantName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<TenantName>(TenantErrors.Name.Empty);
        if (value.Length > 100)
            return Result.Failure<TenantName>(TenantErrors.Name.TooLong);
        return Result.Success(new TenantName(value.Trim()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
