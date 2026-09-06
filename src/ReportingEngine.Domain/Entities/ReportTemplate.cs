using Smbc.Risk.Core.Domain.Shared.Entities;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportTemplate : EntityBase
{
    public long ReportMasterId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string UploadedFileName { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public int TemplateVersion { get; set; } = 1;
    public string? DefinedNameFilters { get; set; } = string.Empty;

    public ReportMaster? ReportMaster { get; set; }
    public ICollection<ReportMetric> ReportMetrics { get; set; } = [];
}
