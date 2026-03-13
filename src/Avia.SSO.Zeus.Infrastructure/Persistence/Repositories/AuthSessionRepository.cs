using Dapper;
using Avia.SSO.Zeus.Domain.Session.Aggregates;
using Avia.SSO.Zeus.Domain.Session.Repositories;
using Avia.SSO.Zeus.Domain.Session.ValueObjects;

namespace Avia.SSO.Zeus.Infrastructure.Persistence.Repositories;

public sealed class AuthSessionRepository(IDbConnectionFactory connectionFactory) : IAuthSessionRepository
{
    public async Task<AuthSession?> GetByIdAsync(SessionId id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT id, user_id, tenant_id, user_agent, ip_address, expires_at, is_revoked, created_at
            FROM auth_sessions
            WHERE id = @Id
            """;

        using var conn = connectionFactory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<AuthSessionRow>(sql, new { Id = id.Value });
        return row?.ToDomain();
    }

    public async Task<IEnumerable<AuthSession>> GetActiveByUserAsync(Guid userId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT id, user_id, tenant_id, user_agent, ip_address, expires_at, is_revoked, created_at
            FROM auth_sessions
            WHERE user_id = @UserId AND is_revoked = false AND expires_at > NOW()
            """;

        using var conn = connectionFactory.Create();
        var rows = await conn.QueryAsync<AuthSessionRow>(sql, new { UserId = userId });
        return rows.Select(r => r.ToDomain());
    }

    public async Task AddAsync(AuthSession session, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO auth_sessions (id, user_id, tenant_id, user_agent, ip_address, expires_at, is_revoked, created_at)
            VALUES (@Id, @UserId, @TenantId, @UserAgent, @IpAddress, @ExpiresAt, @IsRevoked, @CreatedAt)
            """;

        using var conn = connectionFactory.Create();
        await conn.ExecuteAsync(sql, new
        {
            Id = session.SessionId.Value,
            session.UserId,
            TenantId = session.TenantId.Value,
            UserAgent = session.DeviceInfo.UserAgent,
            IpAddress = session.DeviceInfo.IpAddress,
            session.ExpiresAt,
            session.IsRevoked,
            session.CreatedAt
        });
    }

    public async Task UpdateAsync(AuthSession session, CancellationToken ct = default)
    {
        const string sql = "UPDATE auth_sessions SET is_revoked = @IsRevoked WHERE id = @Id";
        using var conn = connectionFactory.Create();
        await conn.ExecuteAsync(sql, new { Id = session.SessionId.Value, session.IsRevoked });
    }

    private sealed record AuthSessionRow(
        Guid Id, Guid UserId, Guid TenantId,
        string UserAgent, string? IpAddress,
        DateTime ExpiresAt, bool IsRevoked, DateTime CreatedAt)
    {
        public AuthSession ToDomain()
        {
            var deviceInfo = Domain.Session.ValueObjects.DeviceInfo.Create(UserAgent, IpAddress);
            var result = AuthSession.Create(UserId, TenantId, deviceInfo);
            return result.Value;
        }
    }
}
