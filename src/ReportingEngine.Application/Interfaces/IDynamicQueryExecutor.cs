using System.Data;

namespace Smbc.Risk.ReportingEngine.Application.Interfaces;

public interface IDynamicQueryExecutor
{
    Task<DataTable> ExecuteQueryAsync(
        long? databaseConnectionId,
        string sqlQuery,
        int? maxRows,
        CancellationToken cancellationToken);
}