namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class EnqueueReportRequestDto
{
    public long ReportMasterId { get; set; }
    public string? ParameterValuesJson { get; set; }
    public string? CreatedBy { get; set; }
}
