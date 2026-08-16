using Smbc.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public record ReportMetricDto
{
    public long Id { get; set; }
    public long ReportTemplateId { get; set; }
    public string NamedRange { get; set; } = string.Empty;
    public string SqlQuery { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; } = DatabaseType.SqlServer;
    public int? MaxRows { get; set; }
}
