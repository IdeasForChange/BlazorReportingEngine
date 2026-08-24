using Smbc.Risk.Core.Domain.Shared.Entities;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportMaster : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ReportNamePattern { get; set; } = string.Empty;
    public string ReportDirectory { get; set; } = string.Empty;

    public ICollection<ReportParameter> ReportParameters { get; set; } = [];
    public ICollection<ReportTemplate> ReportTemplates { get; set; } = [];
    public ICollection<ReportRunnerQueue> ReportRunnerQueues { get; set; } = [];
}