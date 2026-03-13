using Avia.SSO.Zeus.Domain.Identity.Enums;

namespace Avia.SSO.Zeus.Domain.Identity.Services;

public interface ITwoFactorService
{
    string GenerateSecret();
    string GenerateCode(string secret);
    bool VerifyCode(string secret, string code, TwoFactorMethod method);
}
