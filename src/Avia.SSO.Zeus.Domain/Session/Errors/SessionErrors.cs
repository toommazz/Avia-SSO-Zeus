using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Session.Errors;

public static class SessionErrors
{
    public static readonly Error NotFound =
        new("Session.NotFound", "Session not found.", ErrorType.NotFound);
    public static readonly Error AlreadyRevoked =
        new("Session.AlreadyRevoked", "Session has already been revoked.", ErrorType.Validation);
    public static readonly Error Expired =
        new("Session.Expired", "Session has expired.", ErrorType.Unauthorized);
}
