using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Smbc.ReportingEngine.Domain.Shared.Enums;
using Smbc.Risk.ReportingEngine.Application.Interfaces;
using System.Data;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Services;

public class DynamicQueryExecutor : IDynamicQueryExecutor
{
    public async Task<DataTable> ExecuteQueryAsync(
        DatabaseType dbType,
        string sqlQuery,
        int? maxRows,
        Dictionary<string, string> parameters,
        CancellationToken cancellationToken)
    {
        var dt = new DataTable();

        // Enforce max row limits directly in SQL if defined
        if (maxRows.HasValue && maxRows.Value > 0)
        {
            sqlQuery = dbType switch
            {
                DatabaseType.SqlServer => $"SELECT TOP {maxRows.Value} * FROM ({sqlQuery}) AS QueryAlias",
                DatabaseType.Postgres or DatabaseType.Sqlite => $"{sqlQuery} LIMIT {maxRows.Value}",
                _ => sqlQuery
            };
        }

        // Standard dynamic execution example for SQL Server & SQLite
        IDbConnection conn = dbType switch
        {
            DatabaseType.Sqlite => new SqliteConnection("Data Source=reports.db"),
            _ => new SqlConnection("Server=localhost;Database=ReportDb;Integrated Security=True;TrustServerCertificate=True;")
        };

        using (conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sqlQuery;

            foreach (var param in parameters)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = param.Key.StartsWith("@") ? param.Key : $"@{param.Key}";
                p.Value = param.Value ?? (object)DBNull.Value;
                cmd.Parameters.Add(p);
            }

            conn.Open();
            using var reader = cmd.ExecuteReader();
            dt.Load(reader);
        }

        return await Task.FromResult(dt);
    }
}