using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Smbc.Risk.ReportingEngine.Application.Interfaces;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;
using System.Data;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Services;

public class DynamicQueryExecutor(
    ILogger<DynamicQueryExecutor> logger,
    IDatabaseConnectionRepository databaseConnectionRepository) : IDynamicQueryExecutor
{
    private readonly ILogger<DynamicQueryExecutor> _logger = logger;
    private readonly IDatabaseConnectionRepository _databaseConnectionRepository = databaseConnectionRepository;

    public async Task<DataTable> ExecuteQueryAsync(
        long? databaseConnectionId,
        string sqlQuery,
        //Dictionary<string, string> parameters,
        int? maxRows,
        CancellationToken cancellationToken)
    {
        if (!databaseConnectionId.HasValue)
        {
            throw new ArgumentException("Database connection ID is required", nameof(databaseConnectionId));
        }

        var databaseConnection = await _databaseConnectionRepository.GetByIdAsync((long)databaseConnectionId, cancellationToken) ?? throw new InvalidOperationException($"Database connection with ID {databaseConnectionId} not found.");

        var dt = new DataTable();

        // Enforce max row limits directly in SQL if defined
        if (maxRows.HasValue && maxRows.Value > 0)
        {
            sqlQuery = databaseConnection.DatabaseType switch
            {
                DatabaseType.SqlServer => $"SELECT TOP {maxRows.Value} * FROM ({sqlQuery}) AS QueryAlias",
                DatabaseType.Postgres or DatabaseType.Sqlite => $"{sqlQuery} LIMIT {maxRows.Value}",
                _ => sqlQuery
            };
        }

        // Standard dynamic execution example for SQL Server & SQLite
        IDbConnection conn = databaseConnection.DatabaseType switch
        {
            DatabaseType.Sqlite => new SqliteConnection($"Data Source={databaseConnection.DatabaseName}"),
            _ => new SqlConnection($"Server={databaseConnection.ServerHost};Database={databaseConnection.DatabaseName};Integrated Security=True;TrustServerCertificate=True;")
        };

        using (conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sqlQuery;

            //foreach (var param in parameters)
            //{
            //    var p = cmd.CreateParameter();
            //    p.ParameterName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
            //    p.Value = param.Value ?? (object)DBNull.Value;
            //    cmd.Parameters.Add(p);
            //}

            conn.Open();
            using var reader = cmd.ExecuteReader();
            dt.Load(reader);
        }

        return await Task.FromResult(dt);
    }
}