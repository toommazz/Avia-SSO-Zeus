using Avia.SSO.Zeus.Domain.Common;
using Avia.SSO.Zeus.Domain.Identity.Enums;

namespace Avia.SSO.Zeus.Domain.Identity.Entities;

public sealed class LoginAttempt : Entity
{
    public bool Succeeded { get; private set; }
    public LoginFailureReason? FailureReason { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime AttemptedAt { get; private set; }

    private LoginAttempt(Guid id) : base(id) { }

    public static LoginAttempt Success(string? ipAddress = null) =>
        new(Guid.NewGuid())
        {
            Succeeded = true,
            IpAddress = ipAddress,
            AttemptedAt = DateTime.UtcNow
        };

    public static LoginAttempt Failure(LoginFailureReason reason, string? ipAddress = null) =>
        new(Guid.NewGuid())
        {
            Succeeded = false,
            FailureReason = reason,
            IpAddress = ipAddress,
            AttemptedAt = DateTime.UtcNow
        };
}
