namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class ReportMasterDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ReportNamePattern { get; set; } = string.Empty;
    public string ReportDirectory { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<ReportParameterDto> ReportParameters { get; set; } = [];
    public List<ReportTemplateDto> ReportTemplates { get; set; } = [];
}
