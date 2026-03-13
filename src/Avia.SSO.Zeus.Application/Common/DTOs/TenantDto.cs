namespace Avia.SSO.Zeus.Application.Common.DTOs;

public sealed record TenantDto(
    Guid Id,
    string Name,
    bool IsActive);
