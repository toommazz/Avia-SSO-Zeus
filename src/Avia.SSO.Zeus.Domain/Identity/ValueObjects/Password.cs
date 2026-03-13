using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Identity.Errors;
using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Identity.ValueObjects;

public sealed class Password : ValueObject
{
    public string Hash { get; }
    public string Salt { get; }

    private Password(string hash, string salt)
    {
        Hash = hash;
        Salt = salt;
    }

    public static Result<Password> Create(string hash, string salt)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return Result.Failure<Password>(UserErrors.Password.Empty);
        if (string.IsNullOrWhiteSpace(salt))
            return Result.Failure<Password>(UserErrors.Password.Empty);
        return Result.Success(new Password(hash, salt));
    }

    public static Password From(string hash, string salt) => new(hash, salt);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
        yield return Salt;
    }
}
