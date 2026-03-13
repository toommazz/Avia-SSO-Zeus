using Dapper;
using Avia.SSO.Zeus.Domain.Multitenancy.Entities;
using Avia.SSO.Zeus.Domain.Multitenancy.Repositories;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;

namespace Avia.SSO.Zeus.Infrastructure.Persistence.Repositories;

public sealed class TenantRepository(IDbConnectionFactory connectionFactory) : ITenantRepository
{
    public async Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT id, name, is_active, created_at,
                   settings_two_factor_required, settings_max_login_attempts, settings_lockout_duration_minutes
            FROM tenants
            WHERE id = @Id
            """;

        using var conn = connectionFactory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<TenantRow>(sql, new { Id = id.Value });
        return row?.ToDomain();
    }

    public async Task<bool> ExistsAsync(TenantId id, CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(1) FROM tenants WHERE id = @Id";
        using var conn = connectionFactory.Create();
        var count = await conn.ExecuteScalarAsync<int>(sql, new { Id = id.Value });
        return count > 0;
    }

    public async Task AddAsync(Tenant tenant, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO tenants (id, name, is_active, created_at,
                settings_two_factor_required, settings_max_login_attempts, settings_lockout_duration_minutes)
            VALUES (@Id, @Name, @IsActive, @CreatedAt,
                @TwoFactorRequired, @MaxLoginAttempts, @LockoutDurationMinutes)
            """;

        using var conn = connectionFactory.Create();
        await conn.ExecuteAsync(sql, new
        {
            Id = tenant.TenantId.Value,
            Name = tenant.Name.Value,
            tenant.IsActive,
            tenant.CreatedAt,
            tenant.Settings.TwoFactorRequired,
            tenant.Settings.MaxLoginAttempts,
            tenant.Settings.LockoutDurationMinutes
        });
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE tenants SET
                name = @Name,
                is_active = @IsActive,
                settings_two_factor_required = @TwoFactorRequired,
                settings_max_login_attempts = @MaxLoginAttempts,
                settings_lockout_duration_minutes = @LockoutDurationMinutes
            WHERE id = @Id
            """;

        using var conn = connectionFactory.Create();
        await conn.ExecuteAsync(sql, new
        {
            Id = tenant.TenantId.Value,
            Name = tenant.Name.Value,
            tenant.IsActive,
            tenant.Settings.TwoFactorRequired,
            tenant.Settings.MaxLoginAttempts,
            tenant.Settings.LockoutDurationMinutes
        });
    }

    private sealed record TenantRow(
        Guid Id, string Name, bool IsActive, DateTime CreatedAt,
        bool SettingsTwoFactorRequired, int SettingsMaxLoginAttempts, int SettingsLockoutDurationMinutes)
    {
        public Tenant ToDomain()
        {
            var result = Tenant.Create(Id, Name);
            return result.Value;
        }
    }
}
