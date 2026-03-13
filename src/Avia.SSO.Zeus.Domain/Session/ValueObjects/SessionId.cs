using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Session.ValueObjects;

public sealed class SessionId : ValueObject
{
    public Guid Value { get; }

    private SessionId(Guid value) => Value = value;

    public static Result<SessionId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Result.Failure<SessionId>(new Error("Session.Id.Empty", "Session ID cannot be empty.", ErrorType.Validation));
        return Result.Success(new SessionId(value));
    }

    public static SessionId New() => new(Guid.NewGuid());

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
