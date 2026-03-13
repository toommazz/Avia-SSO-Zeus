using OtpNet;
using Avia.SSO.Zeus.Domain.Identity.Enums;
using Avia.SSO.Zeus.Domain.Identity.Services;

namespace Avia.SSO.Zeus.Infrastructure.Security;

public sealed class TwoFactorService : ITwoFactorService
{
    public string GenerateSecret()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public string GenerateCode(string secret)
    {
        var key = Base32Encoding.ToBytes(secret);
        var totp = new Totp(key);
        return totp.ComputeTotp();
    }

    public bool VerifyCode(string secret, string code, TwoFactorMethod method)
    {
        if (method != TwoFactorMethod.Totp)
            return string.Equals(secret, code, StringComparison.Ordinal);

        var key = Base32Encoding.ToBytes(secret);
        var totp = new Totp(key);
        return totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));
    }
}
