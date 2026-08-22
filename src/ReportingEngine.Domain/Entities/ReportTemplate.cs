using Smbc.Risk.Core.Domain.Shared.Entities;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportTemplate : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public int TemplateVersion { get; set; } = 1;
    public string ReportDirectory { get; set; } = string.Empty;
    public string ReportNamePattern { get; set; } = string.Empty;

    public ICollection<ReportMetric> Metrics { get; set; } = [];
    public ICollection<ReportParameter> Parameters { get; set; } = [];
    public ICollection<ReportRunnerQueue> QueueItems { get; set; } = [];
}
