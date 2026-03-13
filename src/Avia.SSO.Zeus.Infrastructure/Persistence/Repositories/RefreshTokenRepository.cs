using Dapper;
using Avia.SSO.Zeus.Domain.Identity.Entities;
using Avia.SSO.Zeus.Domain.Identity.Repositories;

namespace Avia.SSO.Zeus.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository(IDbConnectionFactory connectionFactory) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        const string sql = """
            SELECT id, user_id, token, expires_at, is_revoked, created_at
            FROM refresh_tokens
            WHERE token = @Token
            """;

        using var conn = connectionFactory.Create();
        var row = await conn.QuerySingleOrDefaultAsync<RefreshTokenRow>(sql, new { Token = token });
        return row?.ToDomain();
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO refresh_tokens (id, user_id, token, expires_at, is_revoked, created_at)
            VALUES (@Id, @UserId, @Token, @ExpiresAt, @IsRevoked, @CreatedAt)
            """;

        using var conn = connectionFactory.Create();
        await conn.ExecuteAsync(sql, new
        {
            refreshToken.Id,
            refreshToken.UserId,
            refreshToken.Token,
            refreshToken.ExpiresAt,
            refreshToken.IsRevoked,
            refreshToken.CreatedAt
        });
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        const string sql = "UPDATE refresh_tokens SET is_revoked = @IsRevoked WHERE id = @Id";
        using var conn = connectionFactory.Create();
        await conn.ExecuteAsync(sql, new { refreshToken.Id, refreshToken.IsRevoked });
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        const string sql = """
            UPDATE refresh_tokens SET is_revoked = true
            WHERE id IN (
                SELECT rt.id FROM refresh_tokens rt
                INNER JOIN users u ON u.id = @UserId
            )
            """;
        using var conn = connectionFactory.Create();
        await conn.ExecuteAsync(sql, new { UserId = userId });
    }

    private sealed record RefreshTokenRow(Guid Id, Guid UserId, string Token, DateTime ExpiresAt, bool IsRevoked, DateTime CreatedAt)
    {
        public RefreshToken ToDomain() => RefreshToken.Create(UserId, Token, ExpiresAt);
    }
}
