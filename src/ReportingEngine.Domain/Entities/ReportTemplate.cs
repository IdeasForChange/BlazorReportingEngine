using Smbc.Risk.Core.Domain.Shared.Entities;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportTemplate : EntityBase
{
    public long ReportId { get; set; }
    public string TemplateFileName { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public int TemplateVersion { get; set; } = 1;

    public ReportMaster? Report { get; set; }
    public ICollection<ReportMetric> Metrics { get; set; } = new List<ReportMetric>();
}
