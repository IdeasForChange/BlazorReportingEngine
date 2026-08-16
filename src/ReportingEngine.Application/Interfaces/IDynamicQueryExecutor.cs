using Smbc.ReportingEngine.Domain.Shared.Enums;
using System.Data;

namespace Smbc.Risk.ReportingEngine.Application.Interfaces;

public interface IDynamicQueryExecutor
{
    Task<DataTable> ExecuteQueryAsync(
        DatabaseType dbType,
        string sqlQuery,
        int? maxRows,
        Dictionary<string, string> parameters,
        CancellationToken cancellationToken);
}