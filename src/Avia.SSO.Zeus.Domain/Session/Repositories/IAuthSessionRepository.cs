using Avia.SSO.Zeus.Domain.Session.Aggregates;
using Avia.SSO.Zeus.Domain.Session.ValueObjects;

namespace Avia.SSO.Zeus.Domain.Session.Repositories;

public interface IAuthSessionRepository
{
    Task<AuthSession?> GetByIdAsync(SessionId id, CancellationToken ct = default);
    Task<IEnumerable<AuthSession>> GetActiveByUserAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(AuthSession session, CancellationToken ct = default);
    Task UpdateAsync(AuthSession session, CancellationToken ct = default);
}
