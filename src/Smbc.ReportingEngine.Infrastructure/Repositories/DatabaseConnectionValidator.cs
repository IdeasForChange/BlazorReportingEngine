using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using Smbc.ReportingEngine.Domain.Shared.Enums;
using Smbc.ReportingEngine.Domain.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smbc.ReportingEngine.Infrastructure.Repositories;

public class DatabaseConnectionValidator : IDatabaseConnectionValidator
{
    public async Task<(bool IsValid, string? ErrorMessage)> ValidateAsync(DatabaseProvider provider, string connectionString)
    {
        try
        {
            using DbConnection connection = provider switch
            {
                DatabaseProvider.SqlServer => new SqlConnection(connectionString),
                DatabaseProvider.PostgreSQL => new NpgsqlConnection(connectionString),
                DatabaseProvider.MySql => new MySqlConnection(connectionString),
                DatabaseProvider.Sqlite => new SqliteConnection(connectionString),
                DatabaseProvider.Oracle => new OracleConnection(connectionString),
                _ => throw new NotSupportedException($"Provider '{provider}' is not supported.")
            };

            await connection.OpenAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
