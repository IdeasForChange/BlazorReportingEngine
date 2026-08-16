using Smbc.Risk.Core.Domain.Shared.Entities;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportTemplate : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public string OutputDirectory { get; set; } = string.Empty;
    public string FileNamePattern { get; set; } = string.Empty;

    public ICollection<ReportMetric> Metrics { get; set; } = [];
    public ICollection<ReportParameter> Parameters { get; set; } = [];
    public ICollection<ReportRunnerQueue> QueueItems { get; set; } = [];
}
