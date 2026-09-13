using Smbc.Risk.Core.Domain.Shared.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportTemplate : EntityBase
{
    public long ReportMasterId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string UploadedFileName { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public int TemplateVersion { get; set; } = 1;
    public SpreadsheetQueryType QueryType { get; set; } = SpreadsheetQueryType.QueryInCell;
    public string? DefinedNameFilters { get; set; } = string.Empty;
    public ReportMaster? ReportMaster { get; set; }
    public ICollection<ReportMetric> ReportMetrics { get; set; } = new List<ReportMetric>();
    public long? DatabaseConnectionId { get; set; }
    public DatabaseConnection? DatabaseConnection { get; set; }
}
