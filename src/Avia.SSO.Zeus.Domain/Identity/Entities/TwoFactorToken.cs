using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Identity.Enums;

namespace Avia.SSO.Zeus.Domain.Identity.Entities;

public sealed class TwoFactorToken : Entity
{
    public string Code { get; private set; } = null!;
    public TwoFactorMethod Method { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TwoFactorToken(Guid id) : base(id) { }

    public static TwoFactorToken Create(string code, TwoFactorMethod method, int expirationMinutes = 5) =>
        new(Guid.NewGuid())
        {
            Code = code,
            Method = method,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid => !IsUsed && !IsExpired;

    public void MarkAsUsed() => IsUsed = true;
}
