namespace Smbc.Risk.ReportingEngine.Domain.Shared.DataTransferObjects;

public class ReportTemplateDto
{
    public long? Id { get; set; }
    public long ReportId { get; set; }
    public string TemplateFileName { get; set; } = string.Empty;
    public string TemplatePath { get; set; } = string.Empty;
    public int TemplateVersion { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public string FileName { get; set; } = string.Empty;
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();

    public List<ReportMetricDto> Metrics { get; set; } = [];
}