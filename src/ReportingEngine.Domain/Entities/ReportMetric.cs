using Smbc.Risk.Core.Domain.Shared.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportMetric : EntityBase
{
    public string NamedRange { get; set; } = string.Empty;
    public string SqlQuery { get; set; } = string.Empty;
    public int? MaxRows { get; set; }

    public long ReportTemplateId { get; set; }
    public long? DatabaseConnectionId { get; set; }
    public ReportTemplate? ReportTemplate { get; set; }
    public DatabaseConnection? DatabaseConnection { get; set; }
}
