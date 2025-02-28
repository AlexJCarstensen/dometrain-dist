using System.Data;
using Npgsql;

namespace Dometrain.Monolith.Api.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlConnectionFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        return await _dataSource.OpenConnectionAsync(token);
    }
}
