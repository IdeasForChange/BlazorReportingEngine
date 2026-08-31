using System.Data;

namespace Smbc.Risk.ReportingEngine.Application.Interfaces;

public interface IDynamicQueryExecutor
{
    Task<DataTable> ExecuteQueryAsync(
        long? databaseConnectionId,
        string sqlQuery,
        //Dictionary<string, string> parameters,
        int? maxRows,
        CancellationToken cancellationToken);
}