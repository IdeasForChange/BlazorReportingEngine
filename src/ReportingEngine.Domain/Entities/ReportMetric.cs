using Smbc.Risk.Core.Domain.Shared.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportMetric : EntityBase
{
    public long ReportTemplateId { get; set; }
    public string NamedRange { get; set; } = string.Empty;
    public string SqlQuery { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; } = DatabaseType.SqlServer;
    public int? MaxRows { get; set; }

    public ReportTemplate? ReportTemplate { get; set; }
}
