using Avia.SSO.Zeus.Domain.Multitenancy.Entities;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;

namespace Avia.SSO.Zeus.Domain.Multitenancy.Repositories;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct = default);
    Task<bool> ExistsAsync(TenantId id, CancellationToken ct = default);
    Task AddAsync(Tenant tenant, CancellationToken ct = default);
    Task UpdateAsync(Tenant tenant, CancellationToken ct = default);
}
