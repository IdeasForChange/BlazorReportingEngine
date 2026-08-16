using Smbc.ReportingEngine.Domain.Shared.Enums;
using Smbc.Risk.Core.Domain.Shared.Entities;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportMetric : EntityBase
{
    public long ReportTemplateId { get; set; }
    public string NamedRange { get; set; } = string.Empty;
    public string SqlQuery { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; } = DatabaseType.SqlServer;

    // Maximum amount of data to fetch. NULL or -1 means ALL
    public int? MaxRows { get; set; }

    public ReportTemplate? ReportTemplate { get; set; }
}
