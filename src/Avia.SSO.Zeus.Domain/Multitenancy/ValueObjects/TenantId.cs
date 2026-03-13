using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Multitenancy.Errors;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;

public sealed class TenantId : ValueObject
{
    public Guid Value { get; }

    private TenantId(Guid value) => Value = value;

    public static Result<TenantId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Result.Failure<TenantId>(TenantErrors.Id.Empty);
        return Result.Success(new TenantId(value));
    }

    public static TenantId From(Guid value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
