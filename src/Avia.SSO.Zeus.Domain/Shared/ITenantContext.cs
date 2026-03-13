namespace Avia.SSO.Zeus.Domain.Shared;

public interface ITenantContext
{
    Guid TenantId { get; }
}
