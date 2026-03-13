using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Identity.ValueObjects;

public sealed class UserId : ValueObject
{
    public Guid Value { get; }

    private UserId(Guid value) => Value = value;

    public static Result<UserId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Result.Failure<UserId>(new Error("User.Id.Empty", "User ID cannot be empty.", ErrorType.Validation));
        return Result.Success(new UserId(value));
    }

    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(Guid value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
