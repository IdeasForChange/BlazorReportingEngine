namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class ReportRunnerQueueDto
{
    public long Id { get; set; }
    public long ReportMasterId { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? ParameterPayloadJson { get; set; }
    public string? OutputFilePath { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}