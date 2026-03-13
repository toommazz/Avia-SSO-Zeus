using Dapper;
using Avia.SSO.Zeus.Domain.Identity.Aggregates;
using Avia.SSO.Zeus.Domain.Identity.Enums;
using Avia.SSO.Zeus.Domain.Identity.Repositories;
using Avia.SSO.Zeus.Domain.Identity.ValueObjects;
using Avia.SSO.Zeus.Domain.Multitenancy.ValueObjects;

namespace Avia.SSO.Zeus.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default)
    {
        const string sql = """
            SELECT id, tenant_id, email, password_hash, password_salt,
                   two_factor_secret, two_factor_method, status, created_at
            FROM users
            WHERE id = @Id
            """;

        using var conn = connectionFactory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<UserRow>(sql, new { Id = id.Value });
        return row?.ToDomain();
    }

    public async Task<User?> GetByEmailAndTenantAsync(Email email, TenantId tenantId, CancellationToken ct = default)
    {
        const string sql = """
            SELECT id, tenant_id, email, password_hash, password_salt,
                   two_factor_secret, two_factor_method, status, created_at
            FROM users
            WHERE email = @Email AND tenant_id = @TenantId
            """;

        using var conn = connectionFactory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<UserRow>(sql, new { Email = email.Value, TenantId = tenantId.Value });
        return row?.ToDomain();
    }

    public async Task<bool> EmailExistsInTenantAsync(Email email, TenantId tenantId, CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(1) FROM users WHERE email = @Email AND tenant_id = @TenantId";
        using var conn = connectionFactory.Create();
        var count = await conn.ExecuteScalarAsync<int>(sql, new { Email = email.Value, TenantId = tenantId.Value });
        return count > 0;
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO users (id, tenant_id, email, password_hash, password_salt,
                two_factor_secret, two_factor_method, status, created_at)
            VALUES (@Id, @TenantId, @Email, @PasswordHash, @PasswordSalt,
                @TwoFactorSecret, @TwoFactorMethod, @Status, @CreatedAt)
            """;

        using var conn = connectionFactory.Create();
        await conn.ExecuteAsync(sql, new
        {
            Id = user.UserId.Value,
            TenantId = user.TenantId.Value,
            Email = user.Email.Value,
            PasswordHash = user.Password.Hash,
            PasswordSalt = user.Password.Salt,
            TwoFactorSecret = user.TwoFactorSecret?.Value,
            TwoFactorMethod = user.TwoFactorMethod.ToString(),
            Status = user.Status.ToString(),
            user.CreatedAt
        });
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE users SET
                email = @Email,
                password_hash = @PasswordHash,
                password_salt = @PasswordSalt,
                two_factor_secret = @TwoFactorSecret,
                two_factor_method = @TwoFactorMethod,
                status = @Status
            WHERE id = @Id
            """;

        using var conn = connectionFactory.Create();
        await conn.ExecuteAsync(sql, new
        {
            Id = user.UserId.Value,
            Email = user.Email.Value,
            PasswordHash = user.Password.Hash,
            PasswordSalt = user.Password.Salt,
            TwoFactorSecret = user.TwoFactorSecret?.Value,
            TwoFactorMethod = user.TwoFactorMethod.ToString(),
            Status = user.Status.ToString()
        });
    }

    private sealed record UserRow(
        Guid Id, Guid TenantId, string Email,
        string PasswordHash, string PasswordSalt,
        string? TwoFactorSecret, string TwoFactorMethod,
        string Status, DateTime CreatedAt)
    {
        public User ToDomain() => User.Reconstitute(
            Id, TenantId, Email,
            PasswordHash, PasswordSalt,
            TwoFactorSecret,
            Enum.Parse<Domain.Identity.Enums.TwoFactorMethod>(TwoFactorMethod),
            Enum.Parse<Domain.Identity.Enums.UserStatus>(Status),
            CreatedAt);
    }
}
