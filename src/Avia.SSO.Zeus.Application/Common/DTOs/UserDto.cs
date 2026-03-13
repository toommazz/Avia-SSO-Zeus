namespace Avia.SSO.Zeus.Application.Common.DTOs;

public sealed record UserDto(
    Guid Id,
    Guid TenantId,
    string Email,
    string Status,
    bool TwoFactorEnabled);
