using System.Data;
using Npgsql;

namespace Avia.SSO.Zeus.Infrastructure.Persistence;

public sealed class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection Create() => new NpgsqlConnection(connectionString);
}
