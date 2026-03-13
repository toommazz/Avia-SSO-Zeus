namespace Avia.SSO.Zeus.Application.Common.DTOs;

public sealed record AuthTokenDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresInMinutes);
