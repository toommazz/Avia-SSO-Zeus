namespace Avia.SSO.Zeus.Domain.Identity.Services;

public interface IPasswordHasher
{
    (string Hash, string Salt) Hash(string plainPassword);
    bool Verify(string plainPassword, string hash, string salt);
}
