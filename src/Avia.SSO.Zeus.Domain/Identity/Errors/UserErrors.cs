using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Domain.Identity.Errors;

public static class UserErrors
{
    public static class Email
    {
        public static readonly Error Empty =
            new("User.Email.Empty", "Email cannot be empty.", ErrorType.Validation);
        public static readonly Error InvalidFormat =
            new("User.Email.InvalidFormat", "Invalid email format.", ErrorType.Validation);
        public static readonly Error AlreadyInUse =
            new("User.Email.AlreadyInUse", "Email already in use.", ErrorType.Conflict);
    }

    public static class Password
    {
        public static readonly Error TooShort =
            new("User.Password.TooShort", "Password must be at least 8 characters.", ErrorType.Validation);
        public static readonly Error NoUpperCase =
            new("User.Password.NoUpperCase", "Password must contain at least one uppercase letter.", ErrorType.Validation);
        public static readonly Error NoSpecialChar =
            new("User.Password.NoSpecialChar", "Password must contain at least one special character.", ErrorType.Validation);
        public static readonly Error Empty =
            new("User.Password.Empty", "Password cannot be empty.", ErrorType.Validation);
    }

    public static readonly Error NotFound =
        new("User.NotFound", "User not found.", ErrorType.NotFound);
    public static readonly Error LockedOut =
        new("User.LockedOut", "Account locked due to too many failed attempts.", ErrorType.Forbidden);
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid credentials.", ErrorType.Unauthorized);
    public static readonly Error TwoFactorRequired =
        new("User.TwoFactorRequired", "Two-factor authentication required.", ErrorType.Unauthorized);
    public static readonly Error TwoFactorInvalid =
        new("User.TwoFactorInvalid", "Invalid or expired 2FA code.", ErrorType.Unauthorized);
    public static readonly Error AlreadyDeactivated =
        new("User.AlreadyDeactivated", "User is already deactivated.", ErrorType.Validation);
}
