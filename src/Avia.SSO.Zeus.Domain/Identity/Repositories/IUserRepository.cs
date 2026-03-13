using Avia.SSO.Zeus.Domain.Identity.Aggregates;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;

namespace Avia.SSO.Zeus.Domain.Identity.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default);
    Task<User?> GetByEmailAndTenantAsync(Email email, TenantId tenantId, CancellationToken ct = default);
    Task<bool> EmailExistsInTenantAsync(Email email, TenantId tenantId, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
}
