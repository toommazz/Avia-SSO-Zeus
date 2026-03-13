using Avia.SSO.Zeus.Domain.Identity.Aggregates;

namespace Avia.SSO.Zeus.Domain.Identity.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
