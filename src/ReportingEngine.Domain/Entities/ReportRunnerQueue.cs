using Smbc.Risk.Core.Domain.Shared.Entities;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Domain.Entities;

public class ReportRunnerQueue : EntityBase
{
    public QueueStatus Status { get; set; } = QueueStatus.Pending;
    public string ParameterValuesJson { get; set; } = "{}"; // JSON map of parameter values
    public int ProgressPercentage { get; set; }
    public string? OutputFilePath { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    public long ReportMasterId { get; set; }
    public ReportMaster ReportMaster { get; set; } = null!;
}
