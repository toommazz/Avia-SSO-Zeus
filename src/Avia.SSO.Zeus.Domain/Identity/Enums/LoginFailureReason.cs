namespace Avia.SSO.Zeus.Domain.Identity.Enums;

public enum LoginFailureReason
{
    InvalidPassword,
    UserNotFound,
    AccountLocked,
    AccountDeactivated,
    TwoFactorRequired,
    TwoFactorInvalid
}
