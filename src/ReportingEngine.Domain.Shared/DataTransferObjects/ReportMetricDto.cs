namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public record ReportMetricDto
{
    public long Id { get; set; }
    public long ReportTemplateId { get; set; }
    public string NamedRange { get; set; } = string.Empty;
    public long? DatabaseConnectionId { get; set; }
    public string SqlQuery { get; set; } = string.Empty;
    public int? MaxRows { get; set; }
    public bool IsActive { get; set; } = true;
}
