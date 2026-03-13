using System.Data;

namespace Avia.SSO.Zeus.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    IDbConnection Create();
}
