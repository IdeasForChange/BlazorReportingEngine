namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class CreateReportParameterDto
{
    public long ReportMasterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ParameterType { get; set; } = 1;
    public bool IsRequired { get; set; }

    public ReportMasterDto? ReportMaster { get; set; }
}